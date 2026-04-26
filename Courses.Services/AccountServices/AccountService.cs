using AutoMapper;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO;
using Courses.Core.ModelsDTO.ResponseDTO;
using Courses.Core.Services.Contract;
using Courses.Core.Services.Contract.AccountServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        #region Inject Services

        protected readonly ICreateToken _createToken;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly IEmailSender _emailSender;
        protected readonly ILogger<AccountService> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IMapper _mapper;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ICreateToken createToken, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger, IMapper mapper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _createToken = createToken;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
        }

        #endregion

        #region Register & Confirm Account

        public async Task<ApplicationServiceResult<CreateAccountResponse>> RegisterAsync(CreateAccountRequest req)
        {
            req.Email = req.Email.Trim().ToLower();
            req.UserName = req.UserName.Trim().ToLower();

            var email = await _userManager.FindByEmailAsync(req.Email);
            if (email != null) return ApplicationServiceResult<CreateAccountResponse>.Fail("This Email Is Already Exist");
            var userName = await _userManager.FindByNameAsync(req.UserName);
            if(userName != null) return ApplicationServiceResult<CreateAccountResponse>.Fail("This User Name Is Already Exist");

            var user = _mapper.Map<ApplicationUser>(req);

            try
            {
                var result = await _userManager.CreateAsync(user, req.Password);
                // If Account Not Created
                if (!result.Succeeded)
                    return ApplicationServiceResult<CreateAccountResponse>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

                // Create Token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = WebUtility.UrlEncode(token);
                var frontUrl = _configuration["FrontEndUrl"];
                var routeUrl = _configuration["FrontEndRoute:ConfirmAccount"];
                // Create Link
                var link = $"{frontUrl}/{routeUrl}?userId={user.Id}&token={token}";
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Confirm Your Account",
                    $"<p>To Confirm your account click <a href='{link}'>Here</a></p>"
                    );
                var data = _mapper.Map<CreateAccountResponse>(user);
                return ApplicationServiceResult<CreateAccountResponse>.Success(data, "Account Created Succeeded You Received Confirm Email");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CreateAccountResponse>.Fail("There is error in database");
            }
        }

        public async Task<ApplicationServiceResult<ConfirmAccountResponse>> ConfirmAccount(ConfirmAccountRequest req)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(req.UserId);
                if (user == null) return ApplicationServiceResult<ConfirmAccountResponse>.Fail("Invalid Account");
                
                var decodedToken = WebUtility.UrlDecode(req.Token);
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                if (!result.Succeeded) return ApplicationServiceResult<ConfirmAccountResponse>.Fail("Invalid or expired token");
                var response = new ConfirmAccountResponse
                {
                    IsActivated = true
                };
                return ApplicationServiceResult<ConfirmAccountResponse>.Success(response, "Account activated Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<ConfirmAccountResponse>.Fail("There is error in database");
            }
        }

        #endregion

        #region Login Async
        public Task<ApplicationServiceResult<LoginResponse>> LoginAsync(LoginRequest req)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CheckAccountAsync
        public Task<ApplicationServiceResult<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest req)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CheckOTP
        public Task<ApplicationServiceResult<CheckOTPResponse>> CheckOTP(CheckOTPRequest req)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ResetPasswordAsync
        public Task<ApplicationServiceResult<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest req)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

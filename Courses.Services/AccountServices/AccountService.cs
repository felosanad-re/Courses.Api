using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO;
using Courses.Core.ModelsDTO.ResponseDTO;
using Courses.Core.RedisRepository;
using Courses.Core.Services.Contract;
using Courses.Core.Services.Contract.AccountServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

namespace Courses.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        #region Inject Services

        protected readonly ICreateToken _createToken;
        protected readonly IRedisRepo<OTPUser> _redisRepo;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly SignInManager<ApplicationUser> _signInManager;
        protected readonly IEmailSender _emailSender;
        protected readonly ILogger<AccountService> _logger;
        protected readonly IConfiguration _configuration;
        protected readonly IMapper _mapper;

        public AccountService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, ICreateToken createToken, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger, IMapper mapper, IRedisRepo<OTPUser> redisRepo)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _createToken = createToken;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _redisRepo = redisRepo;
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
                // Add Account Role
                await _userManager.AddToRoleAsync(user, Roles.Student);
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
        public async Task<ApplicationServiceResult<LoginResponse>> LoginAsync(LoginRequest req)
        {
            try
            {
                var user = req.UserNameOrEmail.Contains('@') ?
                await _userManager.FindByEmailAsync(req.UserNameOrEmail):
                await _userManager.FindByNameAsync(req.UserNameOrEmail);

                if (user == null) return ApplicationServiceResult<LoginResponse>.Fail("User Not Found");

                var result = await _signInManager.PasswordSignInAsync(
                    user,
                    req.Password,
                    req.RememberMe,
                    true
                    );
                if (result.IsLockedOut) return ApplicationServiceResult<LoginResponse>.Fail("Email Is Lockout");
                if (result.IsNotAllowed) return ApplicationServiceResult<LoginResponse>.Fail("Email Not Confirm");
                if (!result.Succeeded) return ApplicationServiceResult<LoginResponse>.Fail("Invalid email or password");

                var token = await _createToken.CreateTokenAsync(user);
                var roles = await _userManager.GetRolesAsync(user);
                var data = new LoginResponse()
                {
                    Email = user.Email!,
                    UserName = user.UserName!,
                    IsAuthenticated = true,
                    Roles = roles,
                    Token = token
                };
                return ApplicationServiceResult<LoginResponse>.Success(data, "Login Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for user {UserName}. Error: {Error}", req.UserNameOrEmail, ex.Message);
                return ApplicationServiceResult<LoginResponse>.Fail("There is error in database");
            }
        }
        #endregion

        #region CheckAccountAsync
        public Task<ApplicationServiceResult<CheckAccountResponse>> CheckAccountAsync(CheckAccountRequest req)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CheckOTP
        public async Task<ApplicationServiceResult<CheckOTPResponse>> CheckOTP(CheckOTPRequest req)
        {
            try
            {
                var key = $"OTP:{req.Token}";
                var userOTP = await _redisRepo.GetKeyAsync(key);

                if (userOTP == null)
                    return ApplicationServiceResult<CheckOTPResponse>.Fail("Invalid or expired token, please request a new OTP");

                if (userOTP.IsExpired(TimeSpan.FromMinutes(5)))
                {
                    await _redisRepo.DeleteKeyAsync(key);
                    return ApplicationServiceResult<CheckOTPResponse>.Fail("OTP has expired, please request a new one");
                }

                if (userOTP.OTP != req.OTP)
                    return ApplicationServiceResult<CheckOTPResponse>.Fail("Invalid OTP code");

                // Mark as verified and update in Redis
                userOTP.IsVerified = true;
                await _redisRepo.SetKeyAsync(key, userOTP, TimeSpan.FromMinutes(10));

                var result = new CheckOTPResponse
                {
                    IsValid = true,
                    Token = userOTP.Token
                };
                return ApplicationServiceResult<CheckOTPResponse>.Success(result, "OTP verified successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckOTP failed. Error: {Error}", ex.Message);
                return ApplicationServiceResult<CheckOTPResponse>.Fail("There is error in database");
            }
        }
        #endregion

        #region ResetPasswordAsync
        public async Task<ApplicationServiceResult<ResetPasswordResponse>> ResetPasswordAsync(ResetPasswordRequest req)
        {
            try
            {
                var key = $"OTP:{req.Token}";
                var userOTP = await _redisRepo.GetKeyAsync(key);

                if (userOTP == null)
                    return ApplicationServiceResult<ResetPasswordResponse>.Fail("Invalid or expired token, please start over");

                if (!userOTP.IsVerified)
                    return ApplicationServiceResult<ResetPasswordResponse>.Fail("OTP not verified, please verify your OTP first");

                var user = await _userManager.FindByIdAsync(userOTP.UserId);
                if (user == null)
                    return ApplicationServiceResult<ResetPasswordResponse>.Fail("User not found");

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, req.Password);

                if (!result.Succeeded)
                    return ApplicationServiceResult<ResetPasswordResponse>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

                // Clean up OTP from Redis after successful reset
                await _redisRepo.DeleteKeyAsync(key);

                var response = new ResetPasswordResponse
                {
                    UserId = user.Id,
                    Email = user.Email!
                };
                return ApplicationServiceResult<ResetPasswordResponse>.Success(response, "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ResetPassword failed. Error: {Error}", ex.Message);
                return ApplicationServiceResult<ResetPasswordResponse>.Fail("There is error in database");
            }
        }

        #endregion

        #region Private Helper Methods
        private async Task<string> GenerateAndStoreOTPAsync(string userId, string email)
        {
            var OTP = RandomNumberGenerator.GetInt32(100000, 999999); // 6 Digits
            var token = Guid.NewGuid().ToString("N");
            var userOTP = new OTPUser
            {
                OTP = OTP,
                Token = token,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // Set OTP In Redis using Token as key (so CheckOTP can look it up)
            var key = $"OTP:{token}";
            await _redisRepo.SetKeyAsync(key, userOTP, TimeSpan.FromMinutes(5));

            var htmlMessage = $@"
                <h2>Password Reset Request</h2>
                <p>Your OTP Code: <strong>{OTP}</strong></p>
                <p><small>This code expires in 5 minutes.</small></p>";

            await _emailSender.SendEmailAsync(email, "Forget Password", htmlMessage);

            return token;
        }
        #endregion
    }
}

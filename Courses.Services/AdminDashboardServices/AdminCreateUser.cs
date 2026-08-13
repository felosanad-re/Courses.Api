using AutoMapper;
using Courses.Core;
using Courses.Core.Models.ApplicationUsers;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Account.AdminManagementAccounts;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminCreateUser : IAdminCreateUser
    {
        #region Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<AdminCreateUser> _logger;
        protected readonly IMapper _mapper;

        public AdminCreateUser(UserManager<ApplicationUser> userManager, ILogger<AdminCreateUser> logger, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        #endregion

        public async Task<ApplicationServiceResult<bool>> CreateUserAsync(AdminCreateUserReq req)
        {
            const string succeededMessage = "Account Created Successfully";
            const string ErrorMessage = "There is no user with this id";
            const string LoggerMessage = "There is problem in database";
            const string UserNameWarrningMessage = "There is Account with this user name";
            const string EmailWarrningMessage = "There is Account with this email";

            string? adminId = null;

            try
            {
                adminId = _currentUserService.UserId;
                if (adminId is null)
                    return ApplicationServiceResult<bool>.Fail(ErrorMessage);

                if (req.Role != Roles.Admin && req.Role != Roles.Instructor)
                    return ApplicationServiceResult<bool>.Fail("Invalid role.");

                var exsistingUserName = await _userManager.FindByNameAsync(req.UserName);
                if (exsistingUserName is not null)
                    return ApplicationServiceResult<bool>.Fail(UserNameWarrningMessage);

                var exsistingEmail = await _userManager.FindByEmailAsync(req.Email);
                if (exsistingEmail is not null)
                    return ApplicationServiceResult<bool>.Fail(EmailWarrningMessage);

                var user = _mapper.Map<ApplicationUser>(req);

                var res = await _userManager.CreateAsync(user, req.Password);
                if (!res.Succeeded)
                    return ApplicationServiceResult<bool>.Fail(string.Join(", ", res.Errors.Select(e => e.Description)));

                // Add Role to user [Admin | Instructor]
                var roleRes = await _userManager.AddToRoleAsync(user, req.Role);
                if (!roleRes.Succeeded)
                    return ApplicationServiceResult<bool>.Fail(string.Join(", ", roleRes.Errors.Select(e => e.Description)));

                if (req.Role == Roles.Instructor)
                {
                    var instructor = new Instructor
                    {
                        UserId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = adminId,
                        Status = InstructorStatus.Approved,
                        ApprovedAt = DateTime.UtcNow,
                        Specialization = req.Specialization
                    };

                    await _unitOfWork.CreateRepository<Instructor>().AddAsync(instructor);
                    await _unitOfWork.CompleteAsync();
                }

                return ApplicationServiceResult<bool>.Success(true, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to create user by adminId {adminId}", adminId);
                return ApplicationServiceResult<bool>.Fail(LoggerMessage);
            }
        }
    }
}

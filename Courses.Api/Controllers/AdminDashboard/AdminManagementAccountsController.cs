using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Account.AdminManagementAccounts;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.AdminDashboard
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminManagementAccountsController : BaseController
    {
        #region Services
        protected readonly IAdminManagementStudents _adminManagementStudents;
        protected readonly IAdminManagementInstructors _adminManagementInstructors;
        protected readonly IAdminManagementAccountService _adminManagementAccountService;
        protected readonly IAdminCreateUser _adminCreateUser;

        public AdminManagementAccountsController(IAdminManagementStudents adminManagementStudents, IAdminManagementInstructors adminManagementInstructors, IAdminManagementAccountService adminManagementAccountService, IAdminCreateUser adminCreateUser)
        {
            _adminManagementStudents = adminManagementStudents;
            _adminManagementInstructors = adminManagementInstructors;
            _adminManagementAccountService = adminManagementAccountService;
            _adminCreateUser = adminCreateUser;
        }
        #endregion

        #region Get Students
        [HttpGet("Students")] // GET: /api/AdminManagementAccounts/Students
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminWithStudentResponse>>>> GetStudents([FromQuery]StudentParams param)
        {
            var res = await _adminManagementStudents.GetStudentsAsync(param);
            if(!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Student
        [HttpGet("Student/{studentId}")] // GET: /api/AdminManagementAccounts/Student/1
        public async Task<ActionResult<ApplicationServiceResult<AdminWithStudentDetailsResponse>>> GetStudent(int studentId)
        {
            var res = await _adminManagementStudents.GetStudentDetailsAsync(studentId);
            if(!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region GetInstructors
        [HttpGet("Instructors")] //GET: /api/AdminManagementAccounts/Instructors
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminInstructorResponse>>>> GetInstructors([FromQuery] InstructorParams param)
        {
            var res = await _adminManagementInstructors.GetAllInstructorsAsync(param);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region GetInstructor
        [HttpGet("Instructor/{instructorId}")] //GET: /api/AdminManagementAccounts/Instructor/1
        public async Task<ActionResult<ApplicationServiceResult<AdminInstructorDetailsResponse>>> GetInstructor(int instructorId)
        {
            var res = await _adminManagementInstructors.GetInstructorDetailsAsync(instructorId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Suspend Account
        [HttpPatch("{userId}/Suspend")] // PATCH: /api/AdminManagementAccounts/{userId}/Suspend
        public async Task<ActionResult<ApplicationServiceResult<bool>>> SuspendAccount(AccountActionRequest req, string userId)
        {
            var res = await _adminManagementAccountService.SuspendAccountAsync(req, userId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Activate Account
        [HttpPatch("{userId}/Activate")] // PATCH: /api/AdminManagementAccounts/{userId}/Activate
        public async Task<ActionResult<ApplicationServiceResult<bool>>> ActivateAccount(string userId)
        {
            var res = await _adminManagementAccountService.ActivateAccountAsync(userId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Delete Account
        [HttpPatch("{userId}/Delete")] // PATCH: /api/AdminManagementAccounts/{userId}/Delete
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteAccount(AccountActionRequest req, string userId)
        {
            var res = await _adminManagementAccountService.DeleteAccountAsync(req, userId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Restore Account
        [HttpPatch("{userId}/Restore")] // PATCH: /api/AdminManagementAccounts/{userId}/Restore
        public async Task<ActionResult<ApplicationServiceResult<bool>>> RestoreAccount(string userId)
        {
            var res = await _adminManagementAccountService.RestoreAccountAsync(userId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Create Account
        [HttpPost("CreateUser")] // POST: /api/AdminManagementAccounts/CreateUser
        public async Task<ActionResult<ApplicationServiceResult<bool>>> CreateAccount(AdminCreateUserReq req)
        {
            var res = await _adminCreateUser.CreateUserAsync(req);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion
    }
}

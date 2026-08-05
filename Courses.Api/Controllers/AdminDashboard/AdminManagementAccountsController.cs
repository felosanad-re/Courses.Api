using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
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

        public AdminManagementAccountsController(IAdminManagementStudents adminManagementStudents)
        {
            _adminManagementStudents = adminManagementStudents;
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
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminWithStudentResponse>>>> GetStudent(int studentId)
        {
            var res = await _adminManagementStudents.GetStudentDetailsAsync(studentId);
            if(!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion
    }
}

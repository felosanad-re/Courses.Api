using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.AdminDashboard
{
    [Authorize(Roles = Roles.Admin)]
    public class AdminManagementCoursesController : BaseController
    {
        protected readonly IAdminManagementCoursesServices _adminManagementCoursesServices;

        public AdminManagementCoursesController(IAdminManagementCoursesServices adminManagementCoursesServices)
        {
            _adminManagementCoursesServices = adminManagementCoursesServices;
        }

        #region Get Courses
        [HttpGet("Courses")] // GET: /api/AdminManagementCourses/Courses
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminCoursesResponse>>>> GetCourses([FromQuery] CoursesParams param, [FromQuery] CourseType courseType)
        {
            var res = await _adminManagementCoursesServices.GetAllCoursesAsync(param, courseType);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Course Details
        [HttpGet("Course/{courseId}")] // GET: /api/AdminManagementCourses/Course/courseId?type=RecorderCourse
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminCoursesResponse>>>> GetCourseDetails(int courseId, [FromQuery] CourseType type)
        {
            var res = await _adminManagementCoursesServices.GetCourseDetailsAsync(courseId, type);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Update Course Status
        [HttpPatch("{courseId}/status")] // PATCH : /api/AdminManagementCourses/id/status
        public async Task<ActionResult<ApplicationServiceResult<bool>>> UpdateCourseStatus(int courseId, UpdateCourseStatusRequest req)
        {
            var res = await _adminManagementCoursesServices.UpdateCourseStatusAsync(courseId, req);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Delete Course
        [HttpDelete("{courseId}")] // DELETE: /api/AdminManagementCourses/id
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteCourse(int courseId)
        {
            var res = await _adminManagementCoursesServices.DeleteCourseAsync(courseId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion
    }
}

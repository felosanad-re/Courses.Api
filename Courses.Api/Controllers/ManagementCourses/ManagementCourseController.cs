using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.ManagementCourses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.ManagementCourses
{
    [Authorize(Roles = Roles.Instructor)]
    public class ManagementCourseController : ControllerBase
    {
        #region DI Services
        protected readonly IManagementCourse _managementCourse;

        public ManagementCourseController(IManagementCourse managementCourse)
        {
            _managementCourse = managementCourse;
        }
        #endregion

        #region Get Course Details
        [HttpGet("Course/{id}")] // GET: /api/ManagementCourse/Course/id
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> GetCourses(int id)
        {
            var result = await _managementCourse.GetCourseDetailsAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region CreateCourse
        [HttpPost("CreateCourse")] // POST: /api/ManagementCourse/CreateCourse
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> CreateCourseAsync(CreatedCourseRequest req)
        {
            var result = await _managementCourse.CreateCourseAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Update Course
        [HttpPut("UpdateCourse/{id}")] // PUT: /api/ManagementCourse/UpdateCourse/id
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> UpdateCourseAsync([FromRoute] int id, [FromBody] UpdatedCourseRequest req)
        {
            var result = await _managementCourse.UpdateCourseAsync(id, req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Delete Course
        [HttpDelete("Delete/{id}")] // DELETE: /api/ManagementCourse/Delete
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteCourse(int id)
        {
            var result = await _managementCourse.DeleteCourseAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Delete-Courses
        [HttpPost("Delete-Courses")] // POST: /api/ManagementCourse/Delete-Courses
        public async Task<ActionResult<ApplicationServiceResult<DeleteCoursesResult>>> DeleteCourses([FromBody] IEnumerable<int> ids)
        {
            var result = await _managementCourse.DeleteCoursesAsync(ids);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

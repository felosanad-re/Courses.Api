using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Courses.Api.Controllers
{
    [Authorize(Roles = Roles.Instructor)]
    public class InstructorController : BaseController
    {
        protected readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        #region GetAllInstructors
        [HttpGet("Instructors")] // GET: /api/Instructor/Instructors
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<InstructorResponse>>>> GetAllInstructors()
        {
            var result = await _instructorService.GetAllInstructorsAsync();
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Instructor
        [HttpGet("Instructor/{id}")] // GET: /api/Instructor/Instructor/id
        public async Task<ActionResult<ApplicationServiceResult<InstructorResponse>>> GetInstructor(int id)
        {
            var result = await _instructorService.GetInstructorAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Courses for spec Instructor
        [HttpGet("Courses")] // GET: /api/Instructor/Courses
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>>> GetCourses()
        {
            var result = await _instructorService.GetAllCoursesAsync();
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Get Courses Details
        [HttpGet("Course/{id}")] // GET: /api/Instructor/Course/id
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> GetCourses(int id)
        {
            var result = await _instructorService.GetCourseDetailsAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region CreateCourse
        [HttpPost("CreateCourse")] // POST: /api/Instructor/CreateCourse
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> CreateCourseAsync(CreatedCourseRequest req)
        {
            var result = await _instructorService.CreateCourseAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Update Course
        [HttpPut("UpdateCourse/{id}")] // PUT: /api/Instructor/UpdateCourse/id
        public async Task<ActionResult<ApplicationServiceResult<CourseResponseForInstructor>>> UpdateCourseAsync([FromRoute]int id, [FromBody] UpdatedCourseRequest req)
        {
            var result = await _instructorService.UpdateCourseAsync(id, req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Delete Course
        [HttpDelete("Delete/{id}")] // DELETE: /api/Instructor/Delete
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteCourse(int id)
        {
            var result = await _instructorService.DeleteCourseAsync(id);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Delete-Courses
        [HttpPost("Delete-Courses")] // POST: /api/Instructors/Delete-Courses
        public async Task<ActionResult<ApplicationServiceResult<DeleteCoursesResult>>> DeleteCourses([FromBody]IEnumerable<int> ids)
        {
            var result = await _instructorService.DeleteCoursesAsync(ids);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

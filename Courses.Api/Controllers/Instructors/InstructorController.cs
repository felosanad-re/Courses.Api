using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Courses.Api.Controllers.Instructors
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
        public async Task<ActionResult<ApplicationServiceResult<Pagination<CourseResponseForInstructor>>>> GetCourses([FromQuery]CoursesParams param)
        {
            var result = await _instructorService.GetAllCoursesAsync(param);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

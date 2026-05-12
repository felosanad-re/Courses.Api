using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.CoursesServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Courses
{
    public class CoursesController : BaseController
    {
        #region
        protected readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }
        #endregion

        #region Get All Courses Async
        [HttpGet("Courses")] // GET: /api/Courses/Courses
        public async Task<ActionResult<ApplicationServiceResult<Pagination<CoursesToReturnDTO>>>> GetAllCoursesAsync([FromQuery]CoursesParams @params)
        {
            var result = await _courseService.GetAllCoursesAsync(@params);
            if(!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Course Details
        [HttpGet("Course/{courseId}")] // GET: /api/Courses/Course/courseId
        public async Task<ActionResult<CourseDetailsToReturnDTO>> GetCourseDetails(int courseId)
        {
            var result = await _courseService.GetCourseDetailsAsync(courseId);
            if(result is null) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

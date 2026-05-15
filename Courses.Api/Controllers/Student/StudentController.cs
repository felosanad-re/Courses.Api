using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.StudentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Student
{
    public class StudentController : BaseController
    {
        protected readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        #region Get All Student Courses Async
        [HttpGet("Courses")] // GET: /api/Student/Courses
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CoursesToReturnDTO>>>> GetAllCourses()
        {
            var result = await _studentService.GetAllStudentCoursesAsync();
            if (result == null) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

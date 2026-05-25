using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.Services.Contract.StudentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Student
{
    [Authorize(Roles = Roles.Student)]
    public class StudentController : BaseController
    {
        protected readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        #region Get All Student Courses Async
        [HttpGet("Courses")] // GET: /api/Student/Courses
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<EnrollmentWithCoursesResponse>>>> GetAllCourses()
        {
            var result = await _studentService.GetAllStudentCoursesAsync();
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion
    }
}

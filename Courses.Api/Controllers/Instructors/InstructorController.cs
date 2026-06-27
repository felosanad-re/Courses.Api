using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
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

        #region Get My Courses (with enrollment stats)
        [HttpGet("MyCourses")] // GET: /api/Instructor/MyCourses
        public async Task<ActionResult<ApplicationServiceResult<Pagination<InstructorWithCoursesResponse>>>> GetMyCourses([FromQuery] CoursesParams param)
        {
            var result = await _instructorService.GetMyCoursesAsync(param);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Get Students for spec Instructor
        [HttpGet("Students")] // GET: /api/Instructor/Students
        public async Task<ActionResult<ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>>> GetStudents([FromQuery] StudentParams param)
        {
            var result = await _instructorService.GetStudentsInstructorAsync(param);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Get Student
        [HttpGet("student/{id}")] // GET: /api/Instructor/student/id
        public async Task<ActionResult<StudentWithInstructorResponse>> GetStudent(int id)
        {
            var result = await _instructorService.GetStudentInstructorAsync(id);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });
            return Ok(result);
        }
        #endregion

        #region Get Online Courses
        [HttpGet("Online-Courses")] // GET: /api/Instructor/Online-Courses
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CourseTypesResponse>>>> GetOnlineCourses(string? search)
        {
            var result = await _instructorService.GetOnlineCoursesAsync(search);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Online Courses
        [HttpGet("Recorded-Courses")] // GET: /api/Instructor/Recorded-Courses
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<CourseTypesResponse>>>> GetRecordedCourses(string? search)
        {
            var result = await _instructorService.GetRecordedCoursesAsync(search);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Sections
        [HttpGet("Sections/{courseId}")] // Get: /api/instructor/sections/courseId?search=${search}
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<SectionListResponse>>>> GetSections([FromRoute]int courseId)
        {
            var result = await _instructorService.GetSectionsAsync(courseId);
            if (!result.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

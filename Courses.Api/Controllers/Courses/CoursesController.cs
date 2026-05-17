using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.CoursesServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Courses
{
    public class CoursesController : BaseController
    {
        #region
        protected readonly ICourseService _courseService;
        protected readonly ICourseSectionService _courseSectionService;
        protected readonly IVideoCourseService _videoCourseService;

        public CoursesController(
            ICourseService courseService,
            ICourseSectionService courseSectionService,
            IVideoCourseService videoCourseService)
        {
            _courseService = courseService;
            _courseSectionService = courseSectionService;
            _videoCourseService = videoCourseService;
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

        #region Get Course Sections
        [HttpGet("Sections/{courseId}")] // GET: /api/Courses/Sections
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>>> GetSections(int courseId)
        {
            var result = await _courseSectionService.GetAllSections(courseId);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Video Course Endpoints
        [HttpGet("Lecture/{lectureId}/Video")] // GET: /api/Courses/Lecture/{lectureId}/Video
        public async Task<ActionResult<ApplicationServiceResult<CourseWithLectureVideoResponse>>> GetLectureVideo(int lectureId)
        {
            var result = await _videoCourseService.GetLectureVideo(lectureId);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }

        [HttpGet("Section/{sectionId}/NextVideo")] // GET: /api/Courses/Section/{sectionId}/NextVideo
        public async Task<ActionResult<ApplicationServiceResult<CourseWithLectureVideoResponse>>> GetNextLectureVideo(int sectionId, [FromQuery] int currentVideoOrder)
        {
            var result = await _videoCourseService.GetNextLectureVideo(sectionId, currentVideoOrder);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }

        [HttpGet("Section/{sectionId}/PreviousVideo")] // GET: /api/Courses/Section/{sectionId}/PreviousVideo
        public async Task<ActionResult<ApplicationServiceResult<CourseWithLectureVideoResponse>>> GetPreviousLectureVideo(int sectionId, [FromQuery] int currentVideoOrder)
        {
            var result = await _videoCourseService.GetPreviousLectureVideo(sectionId, currentVideoOrder);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

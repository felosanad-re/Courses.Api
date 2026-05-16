using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Progress;
using Courses.Core.ModelsDTO.ResponseDTO.Progress;
using Courses.Core.Services.Contract.ProgressServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Progresses
{
    public class ProgressController : BaseController
    {
        #region DI Services
        protected readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }
        #endregion

        #region Get Lecture Progress
        [HttpGet("StudentProgress/{lectureId}")] // GET: /api/Progress/StudentProgress/lectureId
        public async Task<ActionResult<ApplicationServiceResult<ProgressWithLectureResponse>>> GetLectureProgress(int lectureId)
        {
            var result = await _progressService.GetCurrentLectureProgressAsync(lectureId);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Update Or Add Lecture Progress
        [HttpPost("UpdateProgress")] // POST: /api/Progress/UpdateProgress
        public async Task<ActionResult<ApplicationServiceResult<ProgressWithLectureResponse>>> UpdateOrAddLectureProgress(UpdateAndAddProgressRequest req)
        {
            var result = await _progressService.UpdateAndAddLastWatchedAsync(req);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion

        #region Get Course Progress
        [HttpGet("CourseProgress/{courseId}")]  // GET: /api/Progress/CourseProgress/{courseId}
        public async Task<ActionResult<ApplicationServiceResult<CourseProgressResponse>>> GetCourseProgress(int courseId)
        {
            var result = await _progressService.GetCourseProgressAsync(courseId);
            if (!result.Succeed) return BadRequest(new ErrorResponse(400) { Message = [result.Message] });

            return Ok(result);
        }
        #endregion
    }
}

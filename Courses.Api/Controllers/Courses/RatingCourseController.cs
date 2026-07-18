using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.CoursesServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.Courses
{
    [Authorize(Roles = Roles.Student)]
    public class RatingCourseController : BaseController
    {
        #region
        protected readonly ICourseRatingService _ratingService;

        public RatingCourseController(ICourseRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        #endregion

        #region Create Rating
        [HttpPost("Rating/{courseId}")] // POST: /api/RatingCourse/Rating
        public async Task<ActionResult<ApplicationServiceResult<CourseRatingResponse>>> CreateRating(int courseId, CourseRatingRequest req)
        {
            var res = await _ratingService.CreateCourseRatingAsync(courseId, req);
            if(!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });
            return Ok(res);
        }
        #endregion
    }
}

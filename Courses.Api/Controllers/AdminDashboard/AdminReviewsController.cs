using Courses.Api.ErrorHandler;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Specifications.AdminSpecifications;
using Courses.Services.AdminDashboardServices;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.AdminDashboard
{
    public class AdminReviewsController : BaseController
    {
        protected readonly IAdminReviewsService _adminReviewsService;

        public AdminReviewsController(IAdminReviewsService adminReviewsService)
        {
            _adminReviewsService = adminReviewsService;
        }

        #region Get Reviews
        [HttpGet("Reviews")] // GET: /api/AdminReviews/Reviews
        public async Task<ActionResult<ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>>> GetReviews([FromQuery] ReviewsParams param)
        {
            var res = await _adminReviewsService.GetAllReviewsAsync(param);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Review Details
        [HttpGet("Review/{reviewId}")] // GET: /api/AdminReviews/Review/{reviewId}
        public async Task<ActionResult<ApplicationServiceResult<AdminReviewDetailsResponse>>> GetReviewDetails(int reviewId)
        {
            var res = await _adminReviewsService.GetReviewDetailsAsync(reviewId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Delete Review
        [HttpDelete("{reviewId}/Review")] // Delete: /api/AdminReviews/{reviewId}/Review
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteReview(int reviewId)
        {
            var res = await _adminReviewsService.DeleteReviewAsync(reviewId);
            if (!res.Succeed)
                return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion
    }
}

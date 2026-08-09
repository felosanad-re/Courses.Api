using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.Specifications.AdminSpecifications;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminReviewsService
    {
        Task<ApplicationServiceResult<Pagination<AdminCoursesReviewsResponse>>> GetAllReviewsAsync(ReviewsParams param);
        Task<ApplicationServiceResult<AdminReviewDetailsResponse>> GetReviewDetailsAsync(int reviewId);
        Task<ApplicationServiceResult<bool>> DeleteReviewAsync(int reviewId);
    }
}

using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;

namespace Courses.Core.Services.Contract.AdminDashboardServices
{
    public interface IAdminDashboardService
    {
        Task<ApplicationServiceResult<AdminDashboardStatsResponse>> GetStatsAsync();

        Task<ApplicationServiceResult<AdminDashboardChartsResponse>> GetChartsAsync(DateTime? fromDate, DateTime? toDate);

        Task<ApplicationServiceResult<List<AdminDashboardReviewsResponse>>> GetLatestReviewsAsync();

        Task<ApplicationServiceResult<AdminDashboardQuickActionsResponse>> GetQuickActionsAsync();
    }
}

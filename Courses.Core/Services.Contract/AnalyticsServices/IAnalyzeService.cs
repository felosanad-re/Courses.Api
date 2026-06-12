using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Analyzer;
using Courses.Core.ModelsDTO.ResponseDTO.Analyses;

namespace Courses.Core.Services.Contract.AnalyticsServices
{
    public interface IAnalyzeService
    {
        Task<ApplicationServiceResult<InstructorAnalyticsDto>> GetAnalyzeAsync();

        Task<ApplicationServiceResult<IReadOnlyList<MonthlyAnalyticsDto>>> GetInstructorChartsAnalyticsAsync(ChartRequest req);
    }
}

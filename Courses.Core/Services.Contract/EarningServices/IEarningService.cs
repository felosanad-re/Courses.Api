using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;

namespace Courses.Core.Services.Contract.EarningServices
{
    public interface IEarningService
    {
        Task<ApplicationServiceResult<InstructorEarningStatsResponse>> GetEarningStatsAsync(DateTime? fromDate, DateTime? toDate);

        Task<ApplicationServiceResult<Pagination<InstructorWithEnrollmentsDetails>>> GetInstructorEnrollmentsAsync(EnrollmentsParams param);
    }
}

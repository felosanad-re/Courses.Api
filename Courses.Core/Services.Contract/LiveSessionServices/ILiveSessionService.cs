using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;

namespace Courses.Core.Services.Contract.LiveSessionServices
{
    public interface ILiveSessionService
    {
        // Get Live Sessions Stats
        Task<ApplicationServiceResult<LiveSessionStatisticsResponse>> GetLiveSessionStatsAsync();

        // Get All Live Sessions
        Task<ApplicationServiceResult<Pagination<LiveSessionListResponse>>> GetLiveSessionsAsync(SessionParams param);

        // Get Live Session
        Task<ApplicationServiceResult<LiveSessionDetailsResponse>> GetLiveSessionDetailsAsync(int id);

        // Get Section With Sessions
        Task<ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>> GetSectionsWithSessionsAsync(int courseId);

        // Create Live Session Meeting With Zoom
        Task<ApplicationServiceResult<LiveSessionResponse>> CreateLiveSessionAsync(LiveSessionRequest req);

        // Update
        Task<ApplicationServiceResult<LiveSessionResponse>> UpdatedLiveSessionAsync(LiveSessionRequest req, int sessionId);

        // Delete
        Task<ApplicationServiceResult<bool>> DeletedLiveSessionAsync(int sessionId);
    }
}

using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;

namespace Courses.Core.Services.Contract.LiveSessionServices
{
    public interface ILiveSessionService
    {
        // Get All Live Sessions
        Task<ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>> GetLiveSessionsAsync();

        // Get Live Session
        Task<ApplicationServiceResult<LiveSessionDetailsResponse>> GetLiveSessionDetailsAsync(int id);

        // Create Live Session Meeting With Zoom
        Task<ApplicationServiceResult<LiveSessionResponse>> CreateLiveSessionAsync(LiveSessionRequest req);

        // Update

        // Delete
    }
}

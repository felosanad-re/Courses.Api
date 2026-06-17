using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Zoom;
using Courses.Core.ModelsDTO.ResponseDTO.Zoom;

namespace Courses.Core.Services.Contract.ZoomServices
{
    public interface IZoomService
    {
        /// <summary>
        /// Generates a Zoom Server-to-Server OAuth access token
        /// using the account credentials to authorize API requests.
        /// </summary>
        Task<string> GetAccessToken();

        /// <summary>
        /// Creates a new Zoom meeting for the instructor and returns
        /// the meeting details including the host start URL,
        /// participant join URL, meeting ID, and other related information.
        /// </summary>
        Task<ApplicationServiceResult<ZoomResponse>> CreateMeetingAsync(ZoomRequest req);
    }
}

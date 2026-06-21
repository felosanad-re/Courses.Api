using Courses.Api.ErrorHandler;
using Courses.Core;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.LiveSessionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Api.Controllers.LiveSessions
{
    [Authorize(Roles = Roles.Instructor)]
    public class LiveSessionController : BaseController
    {
        protected readonly ILiveSessionService _liveSessionService;

        public LiveSessionController(ILiveSessionService liveSessionService)
        {
            _liveSessionService = liveSessionService;
        }

        #region GetAllLiveSession
        [HttpGet("LiveSessions")] // GET: /api/LiveSession/LiveSessions
        public async Task<ActionResult<ApplicationServiceResult<LiveSessionListResponse>>> GetAllLiveSession()
        {
            var res = await _liveSessionService.GetLiveSessionsAsync();
            if (!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Session Details
        [HttpGet("{id}")] // GET: /api/LiveSession/id
        public async Task<ActionResult<ApplicationServiceResult<LiveSessionDetailsResponse>>> GetSessionDetails(int id)
        {
            var res = await _liveSessionService.GetLiveSessionDetailsAsync(id);
            if(!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Get Sections With Sessions
        [HttpGet("Sections/Sessions/{courseId}")] // GET: /api/LiveSession/Sections/Sessions
        public async Task<ActionResult<ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>>> GetSectionsWithSessions(int courseId)
        {
            var res = await _liveSessionService.GetSectionsWithSessionsAsync(courseId);
            if(!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });

            return Ok(res);
        }
        #endregion

        #region Create Session
        [HttpPost("CreateSession")] // POST: /api/LiveSession/CreateSession
        public async Task<ActionResult<ApplicationServiceResult<LiveSessionResponse>>> CreateSession(LiveSessionRequest req)
        {
            var res = await _liveSessionService.CreateLiveSessionAsync(req);
            if (!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });
            return Ok(res);
        }
        #endregion

        #region Update Session
        [HttpPost("UpdateLiveSession/{sessionId}")] // POST: /api/LiveSession/UpdateLiveSession/id
        public async Task<ActionResult<ApplicationServiceResult<LiveSessionResponse>>> UpdateSession(int sessionId, LiveSessionRequest req)
        {
            var res = await _liveSessionService.UpdatedLiveSessionAsync(req, sessionId);
            if (!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });
            return Ok(res);
        }
        #endregion

        #region Delete Session
        [HttpDelete("DeleteSession/{sessionId}")] // DELETE: /api/LiveSession/DeleteSesion/id
        public async Task<ActionResult<ApplicationServiceResult<bool>>> DeleteSession(int sessionId)
        {
            var res = await _liveSessionService.DeletedLiveSessionAsync(sessionId);
            if (!res.Succeed) return BadRequest(new ErrorResponse(400) { Message = [res.Message] });
            return Ok(res);
        }
        #endregion
    }
}

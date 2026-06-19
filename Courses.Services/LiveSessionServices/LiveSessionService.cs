using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.LiveSessions;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.LiveSessions;
using Courses.Core.ModelsDTO.RequestDTO.Zoom;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.LiveSessionServices;
using Courses.Core.Services.Contract.ZoomServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.LiveSessionServices
{
    public class LiveSessionService : ILiveSessionService
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IZoomService _zoomService;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly ILogger<LiveSessionService> _logger;
        protected readonly IMapper _mapper;

        public LiveSessionService(IUnitOfWork unitOfWork, IZoomService zoomService, ILogger<LiveSessionService> logger, ICurrentInstructorServices currentInstructorServices, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _zoomService = zoomService;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
            _mapper = mapper;
        }
        #endregion

        #region Create Live Session Async
        /// <summary>
        /// Creates a Zoom meeting and stores it as a live session under a section owned by the current instructor.
        /// </summary>
        public async Task<ApplicationServiceResult<LiveSessionResponse>> CreateLiveSessionAsync(LiveSessionRequest req)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "the meeting Created Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail(errorMessage);

                // The section check also validates ownership through Section.Course.InstructorId.
                var section = await GetInstructorSectionAsync(req.SectionId, instructorId.Value);
                if (section is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail("there is no section with this id for this instructor");

                // Zoom needs the scheduling data before we can persist the local live session.
                var zoomRequest = new ZoomRequest
                {
                    Topic = req.Topic,
                    StartTime = req.ScheduledAt,
                    Duration = req.Duration
                };

                // Do not save locally unless Zoom successfully creates the actual meeting.
                var zoomMeeting = await _zoomService.CreateMeetingAsync(zoomRequest);
                if (zoomMeeting is null || !zoomMeeting.Succeed || zoomMeeting.Data is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail(zoomMeeting?.Message ?? "the zoom meeting not Created");

                var liveSession = new LiveSession
                {
                    SectionId = req.SectionId,
                    Section = section,
                    Topic = req.Topic,
                    HostJoinUrl = zoomMeeting.Data.StartUrl,
                    ZoomMeetingId = zoomMeeting.Data.Id,
                    Status = LiveSessionStatus.Scheduled,
                    StudentJoinUrl = zoomMeeting.Data.JoinUrl,
                    ScheduledAt = req.ScheduledAt,
                    RecordingUrl = null, // Filled later by Zoom webhooks after the recording is ready.
                    DurationMinutes = req.Duration,
                };

                await _unitOfWork.CreateRepository<LiveSession>().AddAsync(liveSession);
                await _unitOfWork.CompleteAsync();

                var liveSessionResponse = _mapper.Map<LiveSessionResponse>(liveSession);
                return ApplicationServiceResult<LiveSessionResponse>.Success(liveSessionResponse, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to create new live Session For instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<LiveSessionResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Live Sessions Async
        /// <summary>
        /// Returns all live sessions created by the current instructor for dashboard/list views.
        /// </summary>
        public async Task<ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>> GetLiveSessionsAsync()
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "you retrieve all Live Sessions Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>.Fail(errorMessage);

                // Reuse the ownership spec so instructors can only see their own sessions.
                var liveSessionSpec = CreateInstructorLiveSessionSpec(instructorId.Value);
                liveSessionSpec.AddOrderBy(x => x.ScheduledAt);

                var liveSessions = await _unitOfWork.CreateRepository<LiveSession>().GetAllAsyncSpec(liveSessionSpec);
                if (!liveSessions.Any())
                    return ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>.Success(new List<LiveSessionListResponse>(), "No Live Session Created Yet");

                var data = _mapper.Map<IReadOnlyList<LiveSessionListResponse>>(liveSessions);

                return ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieved a live session for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<IReadOnlyList<LiveSessionListResponse>>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Live Session Details Async
        /// <summary>
        /// Returns full details for one live session after validating instructor ownership.
        /// </summary>
        public async Task<ApplicationServiceResult<LiveSessionDetailsResponse>> GetLiveSessionDetailsAsync(int id)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "you retrieve live session details Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<LiveSessionDetailsResponse>.Fail(errorMessage);

                var sessionSpec = CreateInstructorLiveSessionSpec(instructorId.Value, id);

                var liveSession = await _unitOfWork.CreateRepository<LiveSession>().GetAsyncSpec(sessionSpec);
                if (liveSession is null)
                    return ApplicationServiceResult<LiveSessionDetailsResponse>.Fail("there is no live session with this id for this instructor");

                var data = _mapper.Map<LiveSessionDetailsResponse>(liveSession);

                return ApplicationServiceResult<LiveSessionDetailsResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve live session details for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<LiveSessionDetailsResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the current instructor id and normalizes invalid auth/current-user states to null.
        /// </summary>
        private async Task<int?> GetCurrentInstructorInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if (instructorInfo is null || !instructorInfo.Succeed || instructorInfo.Data is null)
                return null;

            return instructorInfo.Data.Id;
        }

        /// <summary>
        /// Loads a section only if it belongs to a course owned by the current instructor.
        /// </summary>
        private async Task<Section?> GetInstructorSectionAsync(int sectionId, int instructorId)
        {
            var sectionSpec = new BaseSpecifications<Section>(x =>
                x.Id == sectionId && x.Course.InstructorId == instructorId
            );
            sectionSpec.Includes.Add(x => x.Course);

            return await _unitOfWork.CreateRepository<Section>().GetAsyncSpec(sectionSpec);
        }

        /// <summary>
        /// Builds the common live-session ownership query used by list/details/update/delete operations.
        /// </summary>
        private static BaseSpecifications<LiveSession> CreateInstructorLiveSessionSpec(int instructorId, int? liveSessionId = null)
        {
            var liveSessionSpec = new BaseSpecifications<LiveSession>(x =>
                x.Section.Course.InstructorId == instructorId &&
                (!liveSessionId.HasValue || x.Id == liveSessionId.Value)
            );
            liveSessionSpec.Includes.Add(x => x.Section);
            liveSessionSpec.IncludesString.Add("Section.Course");

            return liveSessionSpec;
        }
        #endregion
    }
}

using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.LiveSessions;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.LiveSessions;
using Courses.Core.ModelsDTO.RequestDTO.Zoom;
using Courses.Core.ModelsDTO.ResponseDTO.LiveSessions;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
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
                const string succeededMessage = "The Live Session Created Succeeded";

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

        #region Get Live Session Stats Async
        public async Task<ApplicationServiceResult<LiveSessionStatisticsResponse>> GetLiveSessionStatsAsync()
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "you retrieve all Live Sessions Statistics Succeeded";
                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<LiveSessionStatisticsResponse>.Fail(errorMessage);

                var totalSessionSpec = CreateInstructorLiveSessionSpec(instructorId.Value);

                var completeSessionSpec = CreateInstructorLiveSessionStatsSpec(
                    instructorId.Value,
                    LiveSessionStatus.Ended);

                var cancelSessionsSpec = CreateInstructorLiveSessionStatsSpec(
                    instructorId.Value,
                    LiveSessionStatus.Cancelled);

                var upcomingSessionsSpec = CreateInstructorLiveSessionStatsSpec(
                    instructorId.Value,
                    LiveSessionStatus.Scheduled,
                    DateTime.UtcNow);

                var sessionRepo = _unitOfWork.CreateRepository<LiveSession>();

                var totalSessions = await sessionRepo.GetCountAsyncSpec(totalSessionSpec);
                var completedSessions = await sessionRepo.GetCountAsyncSpec(completeSessionSpec);
                var cancelSessions = await sessionRepo.GetCountAsyncSpec(cancelSessionsSpec);
                var upcomingSessions = await sessionRepo.GetCountAsyncSpec(upcomingSessionsSpec);

                var data = new LiveSessionStatisticsResponse
                {
                    CancelledSessions = cancelSessions,
                    TotalSessions = totalSessions,
                    CompletedSessions = completedSessions,
                    UpcomingSessions = upcomingSessions
                };

                return ApplicationServiceResult<LiveSessionStatisticsResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieved a live session Statistics for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<LiveSessionStatisticsResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Live Sessions Async
        /// <summary>
        /// Returns all live sessions created by the current instructor for dashboard/list views.
        /// </summary>
        public async Task<ApplicationServiceResult<Pagination<LiveSessionListResponse>>> GetLiveSessionsAsync(SessionParams param)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "you retrieve all Live Sessions Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<Pagination<LiveSessionListResponse>>.Fail(errorMessage);

                // Reuse the ownership spec so instructors can only see their own sessions.
                var liveSessionRepo = _unitOfWork.CreateRepository<LiveSession>();

                var liveSessionSpec = CreateGetAllInstructorLiveSessionSpec(
                    instructorId.Value,
                    param);

                liveSessionSpec.AddOrderBy(x => x.ScheduledAt);
                var liveSessionCountSpec = CreateGetAllInstructorLiveSessionSpec(instructorId.Value, applypagination: false);

                var liveSessions = await liveSessionRepo.GetAllAsyncSpec(liveSessionSpec);

                var liveSessionCount = await liveSessionRepo.GetCountAsyncSpec(liveSessionCountSpec);

                if (!liveSessions.Any())
                    return ApplicationServiceResult<Pagination<LiveSessionListResponse>>.Success(new Pagination<LiveSessionListResponse>(
                        param.PageIndex,
                        param.PageSize,
                        0,
                        new List<LiveSessionListResponse>()
                        ), "No Live Session Created Yet");

                var dataMapping = _mapper.Map<IReadOnlyList<LiveSessionListResponse>>(liveSessions);

                var pagination = new Pagination<LiveSessionListResponse>(
                        param.PageIndex,
                        param.PageSize,
                        liveSessionCount,
                        dataMapping
                    );

                return ApplicationServiceResult<Pagination<LiveSessionListResponse>>.Success(pagination, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieved a live session for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<Pagination<LiveSessionListResponse>>.Fail(loggerError);
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

        #region Get Section with Sessions
        public async Task<ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>> GetSectionsWithSessionsAsync(int courseId)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "you retrieve all Sections with Live Sessions Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>.Fail(errorMessage);

                var sectionsSpec = new BaseSpecifications<Section>(x =>
                    (x.Course.InstructorId == instructorId) &&
                    (x.CourseId == courseId)&&
                    (x.Course.Status == CourseStatus.OnlineCourse)
                );
                sectionsSpec.Includes.Add(x => x.Sessions);

                var sections = await _unitOfWork.CreateRepository<Section>().GetAllAsyncSpec(sectionsSpec);
                if (!sections.Any())
                    return ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>.Success(new List<SectionWithSessionsResponse>(), "there is no sections with online Sessions yet, Create your first Section then Online Sessions");

                var data = _mapper.Map<IReadOnlyList<SectionWithSessionsResponse>>(sections);

                return ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve sections with live session for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<IReadOnlyList<SectionWithSessionsResponse>>.Fail(loggerError);
            }
        }
        #endregion

        #region Updated Live Session Async
        public async Task<ApplicationServiceResult<LiveSessionResponse>> UpdatedLiveSessionAsync(LiveSessionRequest req, int sessionId)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "The Live Session Updated Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail(errorMessage);

                var sessionSpec = CreateInstructorLiveSessionSpec(instructorId.Value, sessionId);
                // Check session is own this instructor
                var sessionRepo = _unitOfWork.CreateRepository<LiveSession>();
                var session = await sessionRepo.GetAsyncSpec(sessionSpec);
                if (session is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail("There is No Session with this Id");

                // Check on Section if instructor want to move session to another Section
                var section = await GetInstructorSectionAsync(req.SectionId, instructorId.Value);
                if (section is null)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail("there is no section with this id for this instructor");

                // Get Zoom Meeting ID
                if (!long.TryParse(session.ZoomMeetingId, out var zoomMeetingId))
                    return ApplicationServiceResult<LiveSessionResponse>.Fail("Invalid Zoom meeting id");

                var zoomData = new ZoomRequest
                {
                    Duration = req.Duration,
                    StartTime = req.ScheduledAt,
                    Topic = req.Topic
                };

                var updateZoomMeeting = await _zoomService.UpdateMeetingAsync(zoomData, zoomMeetingId);
                if (!updateZoomMeeting.Succeed)
                    return ApplicationServiceResult<LiveSessionResponse>.Fail(updateZoomMeeting.Message ?? "Zoom meeting not updated");

                session.SectionId = req.SectionId;
                session.Section = section;
                session.Topic = req.Topic;
                session.ScheduledAt = req.ScheduledAt;
                session.DurationMinutes = req.Duration;

                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<LiveSessionResponse>(session);

                return ApplicationServiceResult<LiveSessionResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to update live Session For instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<LiveSessionResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Deleted Live Session Async
        public async Task<ApplicationServiceResult<bool>> DeletedLiveSessionAsync(int sessionId)
        {
            const string loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                const string errorMessage = "There is no Instructor With This id";
                const string succeededMessage = "The Live Session Deleted Succeeded";

                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<bool>.Fail(errorMessage);

                var sessionRepo = _unitOfWork.CreateRepository<LiveSession>();
                var sessionSpec = CreateInstructorLiveSessionSpec(instructorId.Value, sessionId);
                var session = await sessionRepo.GetAsyncSpec(sessionSpec);
                if (session is null)
                    return ApplicationServiceResult<bool>.Fail("There is No Session with this Id");

                if (!long.TryParse(session.ZoomMeetingId, out var zoomMeetingId))
                    return ApplicationServiceResult<bool>.Fail("Invalid Zoom meeting id");

                var deleteZoom = await _zoomService.DeleteMeetingAsync(zoomMeetingId);
                if (!deleteZoom.Succeed)
                    return ApplicationServiceResult<bool>.Fail(deleteZoom.Message ?? "Zoom meeting not deleted");

                session.Status = LiveSessionStatus.Cancelled;
                sessionRepo.Delete(session);
                await _unitOfWork.CompleteAsync();

                return ApplicationServiceResult<bool>.Success(true, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to deleted live Session For instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<bool>.Fail(loggerError);
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
                (x.Id == sectionId)&&
                (x.Course.InstructorId == instructorId)&&
                (x.Course.Status == CourseStatus.OnlineCourse)
            );
            sectionSpec.Includes.Add(x => x.Course);

            return await _unitOfWork.CreateRepository<Section>().GetAsyncSpec(sectionSpec);
        }

        /// <summary>
        /// Builds the common live-session ownership query used by list/details/update/delete operations.
        /// </summary>
        private static BaseSpecifications<LiveSession> CreateInstructorLiveSessionSpec(int instructorId,  int? liveSessionId = null)
        {
            var liveSessionSpec = new BaseSpecifications<LiveSession>(x =>
                x.Section.Course.InstructorId == instructorId &&
                (!liveSessionId.HasValue || x.Id == liveSessionId.Value)
            );
            liveSessionSpec.Includes.Add(x => x.Section);
            liveSessionSpec.IncludesString.Add("Section.Course");

            return liveSessionSpec;
        }

        private static BaseSpecifications<LiveSession> CreateGetAllInstructorLiveSessionSpec(int instructorId, SessionParams? param = null, bool applypagination = true)
        {
            var search = param?.Search?.Trim().ToLower();
            var spec = new BaseSpecifications<LiveSession>(x =>
                (string.IsNullOrEmpty(search) || x.Topic.ToLower().Contains(search))&&
                (x.Section.Course.InstructorId == instructorId)
            );

            spec.Includes.Add(x => x.Section);
            spec.IncludesString.Add("Section.Course");

            if (applypagination && param is not null)
            {
                spec.AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);

                switch (param?.Sort)
                    {
                        case "CreatedAtAsc":
                            spec.AddOrderBy(x => x.CreatedAt);
                            break;

                        case "CreatedAtDesc":
                            spec.AddOrderByDesc(x => x.CreatedAt);
                            break;

                        default:
                            spec.AddOrderByDesc(x => x.CreatedAt);
                            break;
                    }

            }
            return spec;
        }

        private static BaseSpecifications<LiveSession> CreateInstructorLiveSessionStatsSpec(int instructorId, LiveSessionStatus status, DateTime? scheduledAt = null)
        {
            var spec = new BaseSpecifications<LiveSession>(x => 
                (x.Section.Course.InstructorId == instructorId)&&
                (x.Status == status)&&
                (!scheduledAt.HasValue || x.ScheduledAt >= scheduledAt.Value)
            );

            return spec;
        }
        #endregion
    }
}

using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.Specifications.LectureSpecifications;
using Courses.Core.Specifications.SectionsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ManagementCourses
{
    public class ManagementLecture : IManagementLecture
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly ILogger<ManagementLecture> _logger;
        protected readonly IMapper _mapper;

        public ManagementLecture(IUnitOfWork unitOfWork, ILogger<ManagementLecture> logger, IMapper mapper, ICurrentInstructorServices currentInstructorServices)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentInstructorServices = currentInstructorServices;
        }
        #endregion

        #region Create Lecture Async
        public async Task<ApplicationServiceResult<LectureWithInstructorResponse>> CreateLectureAsync(CreatedLectureRequest req)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no section or u don't have access for this section";
                var succeddedMessage = "The lecture Added Succeeded";

                if (!ValidateLectureInput(req.Title, req.VideoUrl, req.Order, req.DurationInSeconds, out var normalizedVideoUrl, out var validationError))
                    return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(validationError);

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(instructorError);

                // Get Section
                var spec = new SectionWithCourseSpec(req.SectionId, instructorId);

                var section = await _unitOfWork.CreateRepository<Section>().GetAsyncSpec(spec);
                if (section is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                var lecture = _mapper.Map<Lecture>(req);
                lecture.Title = req.Title.Trim();
                lecture.VideoUrl = normalizedVideoUrl;
                lecture.Section.Course.Status = CourseStatus.Draft;
                lecture.Section = section;

                await _unitOfWork.CreateRepository<Lecture>().AddAsync(lecture);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<LectureWithInstructorResponse>(lecture);

                return ApplicationServiceResult<LectureWithInstructorResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to add new lecture in section id {id}", req.SectionId);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Update Lecture Async
        public async Task<ApplicationServiceResult<LectureWithInstructorResponse>> UpdateLectureAsync(UpdatedLectureRequest req)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no lecture or u don't have access for this lecture";
                var succeddedMessage = "The lecture updated Succeeded";

                if (!ValidateLectureInput(req.Title, req.VideoUrl, req.Order, req.DurationInSeconds, out var normalizedVideoUrl, out var validationError))
                    return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(validationError);

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(instructorError);

                var spec = new LectureWithInstructorSpec(req.Id, instructorId);

                var lecture = await _unitOfWork.CreateRepository<Lecture>().GetAsyncSpec(spec);
                if (lecture is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                var section = await _unitOfWork.CreateRepository<Section>()
                    .GetAsyncSpec(new SectionWithCourseSpec(req.SectionId, instructorId));
                if (section is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                _mapper.Map(req, lecture);
                lecture.Title = req.Title.Trim();
                lecture.VideoUrl = normalizedVideoUrl;
                lecture.Section.Course.Status = CourseStatus.Draft;
                lecture.Section = section;
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<LectureWithInstructorResponse>(lecture);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to updated lecture in section id {id}", req.SectionId);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Delete Lecture Async
        public async Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteLectureAsync(int id)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no lecture or u don't have access for this lecture";
                var succeddedMessage = "The lecture deleted Succeeded";

                if (id <= 0) return ApplicationServiceResult<LectureDeletedResponse>.Fail("lecture id must be greater than zero");

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureDeletedResponse>.Fail(instructorError);

                var spec = new LectureWithInstructorSpec(id, instructorId);

                var lecture = await _unitOfWork.CreateRepository<Lecture>().GetAsyncSpec(spec);
                if (lecture is null) return ApplicationServiceResult<LectureDeletedResponse>
                        .Fail(errorMessage);

                lecture.IsDeleted = true;
                await _unitOfWork.CompleteAsync();

                var data = new LectureDeletedResponse()
                {
                    LectureCount = 1,
                    Message = "Lecture deleted successfully"
                };

                return ApplicationServiceResult<LectureDeletedResponse>
                        .Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to delete lecture in lectureId {id}", id);
                return ApplicationServiceResult<LectureDeletedResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Delete Multi Lecture Async
        public async Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteMultiLectureAsync(IEnumerable<int> ids)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no lectures or u don't have access for this lecture";
                var succeddedMessage = "The lectures deleted Succeeded";

                var lectureIds = ids?.Distinct().ToArray() ?? Array.Empty<int>();
                if (lectureIds.Length == 0)
                    return ApplicationServiceResult<LectureDeletedResponse>.Fail("lecture ids are required");

                if (lectureIds.Any(id => id <= 0))
                    return ApplicationServiceResult<LectureDeletedResponse>.Fail("lecture ids must be greater than zero");

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureDeletedResponse>.Fail(instructorError);

                var spec = new LectureWithInstructorSpec(lectureIds, instructorId);

                var lectures = await _unitOfWork.CreateRepository<Lecture>().GetAllAsyncSpec(spec);
                if (!lectures.Any())
                    return ApplicationServiceResult<LectureDeletedResponse>.Fail(errorMessage);

                if (lectures.Count != lectureIds.Length)
                    return ApplicationServiceResult<LectureDeletedResponse>.Fail("some lectures don't exist or you don't have access to them");

                var deletedLectures = lectures.Count;

                foreach (var lecture in lectures)
                {
                    lecture.IsDeleted = true;
                }
                await _unitOfWork.CompleteAsync();

                var data = new LectureDeletedResponse()
                {
                    LectureCount = deletedLectures,
                    Message = $"{deletedLectures} lectures deleted successfully"
                };

                return ApplicationServiceResult<LectureDeletedResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to delete lecture in lecturesId {@LectureIds}", ids);
                return ApplicationServiceResult<LectureDeletedResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Get Lecture Async
        public async Task<ApplicationServiceResult<LectureWithInstructorResponse>> GetLectureAsync(int id)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no lecture or u don't have access for this lecture";
                var succeddedMessage = "you retrieve a lecture succeeded";

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(instructorError);

                var spec = new LectureWithInstructorSpec(id, instructorId);

                var lecture = await _unitOfWork.CreateRepository<Lecture>().GetAsyncSpec(spec);
                if (lecture is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                var data = _mapper.Map<LectureWithInstructorResponse>(lecture);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve a lecture id {id}", id);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Helper Method
        private async Task<int?> GetCurrentInstructor()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if(instructorInfo is null || !instructorInfo.Succeed || instructorInfo.Data is null) return null;

            return instructorInfo.Data.Id;
        }

        private static bool ValidateLectureInput(string? title, string? videoUrl, int order, int durationInSeconds, out string normalizedVideoUrl, out string errorMessage)
        {
            normalizedVideoUrl = string.Empty;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(title))
            {
                errorMessage = "lecture title is required";
                return false;
            }

            if (title.Trim().Length > 300)
            {
                errorMessage = "lecture title can't be more than 300 characters";
                return false;
            }

            if (order <= 0)
            {
                errorMessage = "lecture order must be greater than zero";
                return false;
            }

            if (durationInSeconds <= 0)
            {
                errorMessage = "lecture duration must be greater than zero";
                return false;
            }

            if (string.IsNullOrWhiteSpace(videoUrl))
            {
                errorMessage = "lecture video url is required";
                return false;
            }

            normalizedVideoUrl = videoUrl.Trim();
            if (normalizedVideoUrl.Length > 1000 ||
                !Uri.TryCreate(normalizedVideoUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                errorMessage = "lecture video url must be a valid http or https url";
                return false;
            }

            return true;
        }
        #endregion
    }
}

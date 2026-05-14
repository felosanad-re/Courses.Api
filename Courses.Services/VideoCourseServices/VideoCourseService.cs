using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.Services.Contract.CoursesServices;
using Courses.Core.Specifications.LectureSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.VideoCourseServices
{
    public class VideoCourseService : IVideoCourseService
    {
        #region DI Service
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<VideoCourseService> _logger;

        public VideoCourseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<VideoCourseService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        #region Get Lecture Video
        public async Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetLectureVideo(int lectureId)
        {
            var errorMessage = "there is no lecture with this Id";
            var succeeddedMessage = "Starting your lecture video";
            var logError = "there is a problem in database";

            try
            {
                var lecture = await _unitOfWork.CreateRepository<Lecture>().GetAsync(lectureId);
                if (lecture is null) return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(errorMessage);

                var data = _mapper.Map<CourseWithLectureVideoResponse>(lecture);

                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Success(data, succeeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve video with id: {lectureId}", lectureId);
                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(logError);
            }

        }
        #endregion

        #region Get Next Lecture Video
        public async Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetNextLectureVideo(int sectionId, int currentVideoOrder)
        {
            var nextVideoError = "this last Video In this section";
            var succeeddedMessage = "Starting your lecture video";
            var logError = "there is a problem in database";

            try
            {
                var lectureRepo = _unitOfWork.CreateRepository<Lecture>();
                var nextLectureVideo = await lectureRepo.GetAsyncSpec(new LectureWithNextVideoSpec(sectionId, currentVideoOrder));

                if (nextLectureVideo is null)
                    return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(nextVideoError);

                var data = _mapper.Map<CourseWithLectureVideoResponse>(nextLectureVideo);
                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Success(data, succeeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve next video with sectionId: {sectionId}", sectionId);
                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(logError);
            }
        }
        #endregion

        #region Get Previous Lecture Video
        public async Task<ApplicationServiceResult<CourseWithLectureVideoResponse>> GetPreviousLectureVideo(int sectionId, int currentVideoOrder)
        {
            var succeeddedMessage = "Starting your lecture video";
            var previousVideoError = "this First Video In this section";
            var logError = "there is a problem in database";

            try
            {
                var lectureRepo = _unitOfWork.CreateRepository<Lecture>();
                var previousLectureVideo = await lectureRepo.GetAsyncSpec(new LectureWithPreviousVideoSpec(sectionId, currentVideoOrder));

                if (previousLectureVideo is null)
                    return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(previousVideoError);

                var data = _mapper.Map<CourseWithLectureVideoResponse>(previousLectureVideo);
                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Success(data, succeeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve previous video with sectionId: {sectionId}", sectionId);
                return ApplicationServiceResult<CourseWithLectureVideoResponse>.Fail(logError);
            }
        }
        #endregion

    }
}

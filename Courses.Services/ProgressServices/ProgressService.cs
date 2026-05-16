using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Progress;
using Courses.Core.ModelsDTO.ResponseDTO.Progress;
using Courses.Core.Services.Contract.ProgressServices;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ProgressServices
{
    public class ProgressService : IProgressService
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly IMapper _mapper;
        protected readonly Logger<ProgressService> _logger;

        public ProgressService(IUnitOfWork unitOfWork, IMapper mapper, Logger<ProgressService> logger, ICurrentStudentService currentStudentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _currentStudentService = currentStudentService;
        }
        #endregion

        #region Get Current Lecture Progress Async
        public async Task<ApplicationServiceResult<ProgressWithLectureResponse>> GetCurrentLectureProgressAsync(int lectureId)
        {
            var studentInfo = await ValidateCurrentStudentAsync();
            if (studentInfo is null)
                return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("Student Not Found With this id");
            var studentId = studentInfo.Value.Id;

            try
            {
                var spec = new BaseSpecifications<StudentLectureProgress>(x => x.LectureId == lectureId && x.Enrollment.StudentId == studentId);
                spec.Includes.Add(x => x.Lecture);

                var progress = await _unitOfWork.CreateRepository<StudentLectureProgress>().GetAsyncSpec(spec);

                // if student don't watch lecture yet
                if (progress is null)
                    return ApplicationServiceResult<ProgressWithLectureResponse>.Success(new ProgressWithLectureResponse()
                    {
                        LectureId = lectureId,
                        IsCompleted = false,
                        LastWatchedSeconds = 0
                    }, "No progress found for this lecture");

                var data = _mapper.Map<ProgressWithLectureResponse>(progress);
                return ApplicationServiceResult<ProgressWithLectureResponse>.Success(data, "Retrieved Student Lecture Progress Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Lecture Progress with Id: {lectureId}", lectureId);
                return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Update And Add Last Watched Async
        public async Task<ApplicationServiceResult<ProgressWithLectureResponse>> UpdateAndAddLastWatchedAsync(UpdateAndAddProgressRequest req)
        {
            var studentInfo = await ValidateCurrentStudentAsync();
            if (studentInfo is null)
                return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("Student Not Found With this id");
            var studentId = studentInfo.Value.Id;
            var studentName = studentInfo.Value.Name;

            try
            {
                var progressRepo = _unitOfWork.CreateRepository<StudentLectureProgress>();

                // Fetch lecture — needed for completion check and course identification
                var lecture = await GetLectureWithSectionAsync(req.LectureId);
                if (lecture is null)
                    return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("Lecture not found with this id");

                // Check for existing progress
                var progress = await GetExistingProgressAsync(req.LectureId, studentId);

                if (progress is null)
                {
                    // --- Add new progress ---
                    var courseId = lecture.Section.CourseId;
                    var enrollment = await GetEnrollmentAsync(studentId, courseId);
                    if (enrollment is null)
                        return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("Student is not enrolled in this course");

                    var newProgress = new StudentLectureProgress()
                    {
                        EnrollmentId = enrollment.Id,
                        LectureId = req.LectureId,
                        IsCompleted = false,
                        LastWatchedSeconds = req.CurrentTime,
                        LastAccessedAt = DateTime.UtcNow,
                        CreatedBy = studentName,
                        Lecture = lecture
                    };

                    CheckAndMarkCompletion(newProgress, req.CurrentTime, lecture.DurationInSeconds);

                    await progressRepo.AddAsync(newProgress);
                    await _unitOfWork.CompleteAsync();

                    var newData = _mapper.Map<ProgressWithLectureResponse>(newProgress);
                    return ApplicationServiceResult<ProgressWithLectureResponse>.Success(newData, "Added Student Lecture Progress Successfully");
                }
                else
                {
                    // --- Update existing progress ---
                    progress.LastWatchedSeconds = req.CurrentTime;
                    progress.LastAccessedAt = DateTime.UtcNow;

                    CheckAndMarkCompletion(progress, req.CurrentTime, lecture.DurationInSeconds);

                    await _unitOfWork.CompleteAsync();

                    var data = _mapper.Map<ProgressWithLectureResponse>(progress);
                    return ApplicationServiceResult<ProgressWithLectureResponse>.Success(data, "Updated Student Lecture Progress Successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Lecture Progress with Id: {lectureId}", req.LectureId);
                return ApplicationServiceResult<ProgressWithLectureResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Get Course Progress Async
        public async Task<ApplicationServiceResult<CourseProgressResponse>> GetCourseProgressAsync(int courseId)
        {
            var studentInfo = await ValidateCurrentStudentAsync();
            if (studentInfo is null)
                return ApplicationServiceResult<CourseProgressResponse>.Fail("Student Not Found With this id");
            var studentId = studentInfo.Value.Id;

            try
            {
                // Find enrollment for this student and course
                var enrollmentSpec = new BaseSpecifications<Enrollment>(x => x.StudentId == studentId && x.CourseId == courseId);
                enrollmentSpec.Includes.Add(x => x.LectureProgresses);
                var enrollment = await _unitOfWork.CreateRepository<Enrollment>().GetAsyncSpec(enrollmentSpec);

                if (enrollment is null)
                    return ApplicationServiceResult<CourseProgressResponse>.Fail("Student is not enrolled in this course");

                // Get course with sections and lectures to count total lectures
                var courseSpec = new BaseSpecifications<Course>(x => x.Id == courseId);
                courseSpec.IncludesString.Add("Sections.Lectures");
                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(courseSpec);

                if (course is null)
                    return ApplicationServiceResult<CourseProgressResponse>.Fail("Course not found");

                // Calculate progress metrics
                var totalLectures = course.Sections?.Sum(s => s.Lectures?.Count) ?? 0;
                var completedLectures = enrollment.LectureProgresses?.Count(lp => lp.IsCompleted) ?? 0;
                var progressPercentage = totalLectures > 0
                    ? Math.Round((double)completedLectures / totalLectures * 100, 2)
                    : 0;

                var response = _mapper.Map<CourseProgressResponse>(course);
                response.TotalLectures = totalLectures;
                response.CompletedLectures = completedLectures;
                response.ProgressPercentage = progressPercentage;

                return ApplicationServiceResult<CourseProgressResponse>.Success(response, "Retrieved Course Progress Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve Course Progress with Id: {courseId}", courseId);
                return ApplicationServiceResult<CourseProgressResponse>.Fail("There is a problem in database");
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Validates the current student and returns their ID and Name.
        /// Returns null if the student is not found or invalid.
        /// </summary>
        private async Task<(int Id, string Name)?> ValidateCurrentStudentAsync()
        {
            var student = await _currentStudentService.GetStudentWithApplicationUser();
            if (student is null || !student.Succeed || student.Data is null)
                return null;

            return (student.Data.Id, student.Data.Name);
        }

        /// <summary>
        /// Checks if the watched time exceeds 90% of the lecture duration
        /// and marks the progress as completed if so.
        /// </summary>
        private void CheckAndMarkCompletion(StudentLectureProgress progress, double watchedSeconds, int lectureDurationInSeconds)
        {
            if (watchedSeconds >= lectureDurationInSeconds * 0.9)
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Retrieves the lecture with its Section included, or returns null if not found.
        /// </summary>
        private async Task<Lecture?> GetLectureWithSectionAsync(int lectureId)
        {
            var spec = new BaseSpecifications<Lecture>(x => x.Id == lectureId);
            spec.Includes.Add(x => x.Section);
            return await _unitOfWork.CreateRepository<Lecture>().GetAsyncSpec(spec);
        }

        /// <summary>
        /// Retrieves existing progress for a specific lecture and student,
        /// including Enrollment and Lecture navigation properties.
        /// </summary>
        private async Task<StudentLectureProgress?> GetExistingProgressAsync(int lectureId, int studentId)
        {
            var spec = new BaseSpecifications<StudentLectureProgress>(x => x.LectureId == lectureId && x.Enrollment.StudentId == studentId);
            spec.Includes.Add(x => x.Enrollment);
            spec.Includes.Add(x => x.Lecture);
            return await _unitOfWork.CreateRepository<StudentLectureProgress>().GetAsyncSpec(spec);
        }

        /// <summary>
        /// Finds the enrollment for a student in a specific course.
        /// Returns null if the student is not enrolled.
        /// </summary>
        private async Task<Enrollment?> GetEnrollmentAsync(int studentId, int courseId)
        {
            var spec = new BaseSpecifications<Enrollment>(x => x.StudentId == studentId && x.CourseId == courseId);
            return await _unitOfWork.CreateRepository<Enrollment>().GetAsyncSpec(spec);
        }

        #endregion
    }
}

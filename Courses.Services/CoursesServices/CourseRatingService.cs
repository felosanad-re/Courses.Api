using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.CoursesServices;
using Courses.Core.Services.Contract.StudentServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.CoursesServices
{
    public class CourseRatingService : ICourseRatingService
    {
        #region DI
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentStudentService _currentStudentService;
        protected readonly ILogger<CourseRatingService> _logger;
        protected readonly IMapper _mapper;

        public CourseRatingService(IUnitOfWork unitOfWork, ICurrentStudentService currentStudentService, ILogger<CourseRatingService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentStudentService = currentStudentService;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        public async Task<ApplicationServiceResult<CourseRatingResponse>> CreateCourseRatingAsync(int courseId, CourseRatingRequest req)
        {
            // 1.Validate courseId > 0
            if (courseId < 0)
                return ApplicationServiceResult<CourseRatingResponse>.Fail("Invalid course id.");
            int? studentId = null;
            const string UserNotFoundMessage = "No User Founds";
            const string ErrorMassage = "You Have To Enrollment to this course first";
            const string SucceededMassage = "You Rating This Course Succeeded";
            try
            {
                var studentInfo = await _currentStudentService.GetStudentWithApplicationUser();
                if (studentInfo == null || studentInfo.Data is null)
                    return ApplicationServiceResult<CourseRatingResponse>.Fail(UserNotFoundMessage);
                studentId = studentInfo.Data.Id;
                // 2.Check course exists
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var enrollmentSpec = new EnrollmentWithSpec(studentId.Value, courseId);

                var enrollment = await enrollmentRepo.GetAsyncSpec(enrollmentSpec);
                if (enrollment == null)
                    return ApplicationServiceResult<CourseRatingResponse>.Fail(ErrorMassage);
                var course = enrollment.Course;
                // 3.Check student have rated before [Update Rating]

                // For Course
                var ratingSpec = new BaseSpecifications<CourseRating>(x =>
                        x.StudentId == studentId &&
                        x.CourseId == courseId
                    );

                // To Calculate Total Rating And Count
                var courseRatingSpec = new BaseSpecifications<CourseRating>(x => x.CourseId == courseId);

                var ratingRepo = _unitOfWork.CreateRepository<CourseRating>();

                var existingRating = await ratingRepo.GetAsyncSpec(ratingSpec);
                if (existingRating is not null)
                {
                    // Update rating
                    existingRating.Rating = req.RatingValue;
                    existingRating.Comment = req.Comment;
                }
                else
                {
                    // 4.Create rating
                    var newRating = new CourseRating
                    {
                        Comment = req.Comment,
                        Rating = req.RatingValue,
                        CourseId = courseId,
                        StudentId = studentId.Value
                    };
                    await ratingRepo.AddAsync(newRating);
                    course.RatingCount++;
                }
                await _unitOfWork.CompleteAsync();

                // 5.Update course average & count
                var totalRating = await ratingRepo.GetSumAsyncSpec(courseRatingSpec, x => x.Rating);
                var ratingCount = await ratingRepo.GetCountAsyncSpec(courseRatingSpec);

                course.AverageRating = (decimal)totalRating / ratingCount;
                course.RatingCount = ratingCount;
                // 6.Save changes
                await _unitOfWork.CompleteAsync();

                var response = new CourseRatingResponse
                {
                    CourseId = courseId,
                    Rating = req.RatingValue,
                    Comment = req.Comment,
                    CourseName = course.Name,
                    StudentName = $"{enrollment.Student.ApplicationUser.FirstName} {enrollment.Student.ApplicationUser.LastName}"
                };

                return ApplicationServiceResult<CourseRatingResponse>.Success(response, SucceededMassage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem when student {studentId} try to rating Course {courseId}", studentId, courseId);
                return ApplicationServiceResult<CourseRatingResponse>.Fail("There is a problem in database");
            }
        }
    }
}

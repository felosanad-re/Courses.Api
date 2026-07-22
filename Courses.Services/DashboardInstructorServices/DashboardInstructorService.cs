using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.DashboardInstructor;
using Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor;
using Courses.Core.Services.Contract.DashboardServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
using Courses.Core.Specifications.RatingSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.DashboardInstructorServices
{
    public class DashboardInstructorService : IDashboardInstructorService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<DashboardInstructorService> _logger;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly IMapper _mapper;

        public DashboardInstructorService(IUnitOfWork unitOfWork, ILogger<DashboardInstructorService> logger, ICurrentInstructorServices currentInstructorServices, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
            _mapper = mapper;
        }

        #region Get Dashboard Instruct Stats Async
        public async Task<ApplicationServiceResult<DashboardInstructorDTO>> GetDashboardInstructStatsAsync()
        {
            int? instructorId = null;
            // Get Instructor Id
            try
            {
                instructorId = await GetCurrentInstrurInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<DashboardInstructorDTO>.Fail("there is no instructor with this id");

                // Get All Courses
                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var allCourseSpec = new AllCoursesWithInstructorSpec(instructorId);
                var allCourses = await courseRepo.GetCountAsyncSpec(allCourseSpec);

                // Get New Courses
                var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
                var newCourseSpec = new CoursesCountWithSpec(instructorId.Value, oneMonthAgo);

                var courseSpec = new CoursesCountWithSpec(instructorId.Value);
                var newCourses = await courseRepo.GetCountAsyncSpec(newCourseSpec);

                // Get All Students [Enrollment]
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var enrollmentSpec = new EnrollmentWithSpec(instructorId.Value);

                var allStudents = await enrollmentRepo.GetCountAsyncSpec(enrollmentSpec);

                // Get New Students
                var newEnrollmentSpec = new EnrollmentWithSpec(instructorId.Value, oneMonthAgo);
                var newStudents = await enrollmentRepo.GetCountAsyncSpec(newEnrollmentSpec);

                // Get All Revenues [Enrollment]
                var revenuesSpec = new EnrollmentWithSpec(instructorId);

                var allRevenues = await enrollmentRepo.GetSumAsyncSpec(revenuesSpec, x => x.Amount);

                // Get new Revenues
                var newReveunesSpec = new EnrollmentWithSpec(instructorId.Value, oneMonthAgo);

                var newReveunes = await enrollmentRepo.GetSumAsyncSpec(newReveunesSpec, x => x.Amount);

                // Get Total rating for Every Course
                var totalWeightedRating = await courseRepo.GetSumAsyncSpec(
                    courseSpec,
                    x => x.RatingCount * x.AverageRating);

                var totalRating = await courseRepo.GetSumAsyncSpec(
                    courseSpec,
                    x => x.RatingCount);

                var avarageRating = totalRating == 0 ? 0 : totalWeightedRating / totalRating;

                // Get New rating for Every Course [Work With Course Rating Entity]
                var courseRatingSpec = new BaseSpecifications<CourseRating>(x => 
                    (x.Course.InstructorId == instructorId)&&
                    (x.CreatedAt >= oneMonthAgo)
                );
                var courseRatingRepo = _unitOfWork.CreateRepository<CourseRating>();

                var totalCourseRating = await courseRatingRepo.GetSumAsyncSpec(courseRatingSpec, x => x.Rating);

                var totalRatingCount = await courseRatingRepo.GetCountAsyncSpec(courseRatingSpec);

                var newAvarageRatingInMonth = totalRatingCount == 0 ? 0 : totalCourseRating / totalRatingCount;

                var data = new DashboardInstructorDTO()
                {
                    TotalCourses = allCourses,
                    TotalNewCoursesInMonth = newCourses,
                    TotalStudents = allStudents,
                    NewTotalStudentsInMonth = newStudents,
                    TotalRevenues = allRevenues,
                    NewTotalRevenuesInMonth = newReveunes,
                    AverageRating = avarageRating,
                    NewAverageRatingInMonth = newAvarageRatingInMonth
                };
                return ApplicationServiceResult<DashboardInstructorDTO>.Success(data, "you retrieve all dashboard stats succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is error when try to retrieve status for instructor id  {instructorId}", instructorId);
                return ApplicationServiceResult<DashboardInstructorDTO>.Fail("There Is problem In database");
            }
        }
        #endregion

        public async Task<ApplicationServiceResult<Pagination<DashboardInstructorReviewsDTO>>> GetReviewsAsync(RatingParams param)
        {
            int? instructorId = null;
            const string errorMassage = "No User With This Id";
            const string waringMassage = "No reviews yet.";
            const string succeededMassage = "Reviews retrieved successfully";
            const string loggerError = "there is a problem in database";

            try
            {
                instructorId = await GetCurrentInstrurInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<Pagination<DashboardInstructorReviewsDTO>>.Fail(errorMassage);

                param.Search = param.Search?.Trim().ToLower();
                var ratingSpec = new RatingWithSpec(param, instructorId.Value);
                var ratingCountSpec = new RatingWithSpec(param, instructorId.Value, isCount: true);

                var ratingRepo = _unitOfWork.CreateRepository<CourseRating>();

                var rating = await ratingRepo.GetAllAsyncSpec(ratingSpec);
                if (!rating.Any())
                    return ApplicationServiceResult<Pagination<DashboardInstructorReviewsDTO>>.Success(new Pagination<DashboardInstructorReviewsDTO>(param.PageIndex, param.PageSize, 0, []), waringMassage);
                var ratingCount = await ratingRepo.GetCountAsyncSpec(ratingCountSpec);

                var paginationData = _mapper.Map<IReadOnlyList<DashboardInstructorReviewsDTO>>(rating);

                var data = new Pagination<DashboardInstructorReviewsDTO>(
                        param.PageIndex,
                        param.PageSize,
                        ratingCount,
                        paginationData
                    );
                return ApplicationServiceResult<Pagination<DashboardInstructorReviewsDTO>>.Success(data, succeededMassage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrave the courses reviews for instructorId {instructorId}", instructorId);
                return ApplicationServiceResult<Pagination<DashboardInstructorReviewsDTO>>.Fail(loggerError);
            }
        }

        #region Helper Methods
        private async Task<int?> GetCurrentInstrurInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if(instructorInfo is null || !instructorInfo.Succeed || instructorInfo.Data is null)
                return null;

            return instructorInfo.Data?.Id;
        }
        #endregion
    }
}

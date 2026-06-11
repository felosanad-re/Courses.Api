using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Analyses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.Services.Contract.AnalyticsServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AnalyticsServices
{
    public class AnalyzeService : IAnalyzeService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<AnalyzeService> _logger;
        protected readonly IMapper _mapper;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        public AnalyzeService(IUnitOfWork unitOfWork, ILogger<AnalyzeService> logger, ICurrentInstructorServices currentInstructorServices, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
            _mapper = mapper;
        }

        public async Task<ApplicationServiceResult<InstructorAnalyticsDto>> GetAnalyzeAsync()
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "this all Analytics for instructor";
            var loggerError = "There is a problem in database";
            int? instructorId = null;

            try
            {
                var instructor = await _currentInstructorServices.GetCurrentInstructor();
                if (instructor is null || instructor.Data is null)
                    return ApplicationServiceResult<InstructorAnalyticsDto>.Fail(userNotFoundMessage);
                instructorId = instructor.Data.Id;

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var coursesRepo = _unitOfWork.CreateRepository<Course>();
                var enrollmentSpec = new BaseSpecifications<Enrollment>(x => x.Course.InstructorId == instructorId);
                enrollmentSpec.Includes.Add(x => x.Course);

                var coursesSpec = new BaseSpecifications<Course>(x => x.InstructorId == instructorId);

                var courses = await coursesRepo.GetAllAsyncSpec(coursesSpec);
                if (!courses.Any())
                    return ApplicationServiceResult<InstructorAnalyticsDto>.Success(new InstructorAnalyticsDto
                    {
                        TopCourseRating = null,
                        TopCourseSelling = null,
                        AverageCourseRating = 0,
                        DraftCourses = 0,
                        PublishedCourses = 0,
                        TotalCourses = 0,
                        TotalEnrollments = 0,
                        TotalRevenue = 0,
                        TotalStudents = 0
                    }, succeededMessage);

                var enrollments = await enrollmentRepo.GetAllAsyncSpec(enrollmentSpec);

                // Get Top Best Selling Course
                var bestSellingCourseEnrollments = enrollments
                    .GroupBy(x => x.Course)
                    .Select(group => new
                    {
                        Course = group.Key,
                        Revenue = group.Sum(x => x.Amount),
                        Enrollments = group.Count()
                    }).OrderByDescending(x => x.Enrollments)
                      .FirstOrDefault();

                // Auto Mapping For Image Resolver
                CourseAnalyticDTO? bestSellingCourse = null;
                if(bestSellingCourseEnrollments != null)
                {
                    bestSellingCourse = _mapper.Map<CourseAnalyticDTO>(bestSellingCourseEnrollments.Course);
                    bestSellingCourse.Revenue = bestSellingCourseEnrollments.Revenue;
                    bestSellingCourse.Enrollments = bestSellingCourseEnrollments.Enrollments;
                }

                var data = new InstructorAnalyticsDto
                {
                    TotalCourses = courses.Count,
                    TotalEnrollments = enrollments.Count,
                    TotalStudents = enrollments.Select(x => x.StudentId).Distinct().Count(),
                    TotalRevenue = enrollments.Where(x => x.IsPaid).Sum(x => x.Amount),
                    TopCourseSelling = bestSellingCourse
                };

                return ApplicationServiceResult<InstructorAnalyticsDto>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Analytic For Instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<InstructorAnalyticsDto>.Fail(loggerError);
            }
        }
    }
}

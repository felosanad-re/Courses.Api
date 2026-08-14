using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.ActivitiesServices;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.RatingSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Courses.Services.InstructorServices.ActivitiesServices
{
    public class ActivitiesService : IActivitiesService
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<ActivitiesService> _logger;
        protected readonly ICurrentInstructorServices _currentInstructorServices;

        public ActivitiesService(IUnitOfWork unitOfWork, ILogger<ActivitiesService> logger, ICurrentInstructorServices currentInstructorServices)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentInstructorServices = currentInstructorServices;
        }

        public async Task<ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>> GetActivitiesAsync()
        {
            const string SucceededMessage = "you retrieve all data";
            const string WorningMessage = "there is no data yet";
            const string ErrorMEssage = "No user with this id";
            const string LoogerMessage = "There is problem in database";

            int? instructorId = null;

            try
            {
                var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
                if (instructorInfo == null || instructorInfo.Data == null)
                    return ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>.Fail(ErrorMEssage);

                instructorId = instructorInfo.Data.Id;

                var enrollmentSpec = new BaseSpecifications<Enrollment>(x => x.Course.InstructorId == instructorId.Value);
                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();
                var enrollmentQuery = enrollmentRepo.GetQuerySpec(enrollmentSpec);

                var ratingSpec = new RatingWithSpec(instructorId.Value);
                var ratingRepo = _unitOfWork.CreateRepository<CourseRating>();
                var ratingQuery = ratingRepo.GetQuerySpec(ratingSpec);

                var enrollments = await enrollmentQuery.Select(x => new InstructorActivitiesResponse
                {
                    Amount = x.Amount,
                    CourseTitle = x.Course.Name,
                    CreatedAt = x.CreatedAt,
                    StudentName = x.Student.ApplicationUser.FullName,
                    Type = InstructorActivityType.Enrollment,
                }).OrderByDescending(x => x.CreatedAt).Take(5).ToListAsync();

                var ratings = await ratingQuery.Select(x => new InstructorActivitiesResponse
                {
                    StudentName = x.Student.ApplicationUser.FullName,
                    CreatedAt = x.CreatedAt,
                    CourseTitle = x.Course.Name,
                    Rating = x.Rating,
                    Type = InstructorActivityType.Rating
                }).OrderByDescending(x => x.CreatedAt).Take(5).ToListAsync();


                var activities = enrollments.Concat(ratings).OrderByDescending(x => x.CreatedAt).Take(5).ToList();
                if (!activities.Any())
                    return ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>.Success([], WorningMessage);

                return ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>.Success(activities, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there retrieve data for instructor Id {instructorId}", instructorId);
                return ApplicationServiceResult<IReadOnlyList<InstructorActivitiesResponse>>.Success([], LoogerMessage);
            }
        }
    }
}

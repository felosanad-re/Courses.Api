using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.InstructorsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.InstructorServices
{
    public class InstructorService : IInstructorService
    {
        #region Inject Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ILogger<InstructorService> _logger;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IMapper _mapper;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        #endregion

        #region Get All Instructors Async
        public async Task<ApplicationServiceResult<IReadOnlyList<InstructorResponse>>> GetAllInstructorsAsync()
        {
            var errorMessage = "There is no Instructors yet";
            var succeededMessage = "this all Instructors";
            var loggerError = "There is a problem in database";

            try
            {
                var spec = new InstructorSpec();
                var instructors = await _unitOfWork.CreateRepository<Instructor>().GetAllAsyncSpec(spec);
                if (!instructors.Any()) return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Fail(errorMessage);

                var data = _mapper.Map<IReadOnlyList<InstructorResponse>>(instructors);

                return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<IReadOnlyList<InstructorResponse>>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Instructor Async
        public async Task<ApplicationServiceResult<InstructorResponse>> GetInstructorAsync(int id)
        {
            var errorMessage = "There is no Instructor with this id";
            var succeededMessage = "this Instructor details";
            var loggerError = "There is a problem in database";

            try
            {
                var instructor = await _unitOfWork.CreateRepository<Instructor>().GetAsyncSpec(new InstructorSpec(id));
                if (instructor is null) return ApplicationServiceResult<InstructorResponse>.Fail(errorMessage);

                var data = _mapper.Map<InstructorResponse>(instructor);

                return ApplicationServiceResult<InstructorResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<InstructorResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Get All Courses Async
        public async Task<ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>> GetAllCoursesAsync()
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No Courses For this Instructor";
            var succeededMessage = "this all courses";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>.Fail(userNotFoundMessage);

                var spec = new CourseWithInstructorSpec(userId);
                var courses = await _unitOfWork.CreateRepository<Course>().GetAllAsyncSpec(spec);
                if (!courses.Any()) return ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>.Fail(errorMessage);

                var data = _mapper.Map<IReadOnlyList<CourseResponseForInstructor>>(courses);

                return ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<IReadOnlyList<CourseResponseForInstructor>>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Course Details Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> GetCourseDetailsAsync(int id)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No Courses with this id";
            var succeededMessage = "Course details retrieved successfully";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                var spec = new CourseWithInstructorSpec(id, userId);
                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(spec);
                if (course is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                var data = _mapper.Map<CourseResponseForInstructor>(course);
                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Create Course Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> CreateCourseAsync(CreatedCourseRequest req)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "Instructor not found";
            var succeededMessage = "Course created succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                // Get Current Instructor
                var currentInstructor = await _unitOfWork.CreateRepository<Instructor>()
                    .GetAsyncSpec(new InstructorSpec(userId));
                if (currentInstructor is null)
                    return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                var newCourse = _mapper.Map<Course>(req);
                newCourse.InstructorId = currentInstructor.Id;

                await _unitOfWork.CreateRepository<Course>().AddAsync(newCourse);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<CourseResponseForInstructor>(newCourse);

                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Update Course Async
        public async Task<ApplicationServiceResult<CourseResponseForInstructor>> UpdateCourseAsync(int id, UpdatedCourseRequest req)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No course Found with this id";
            var succeededMessage = "Course updated succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();

                // Get Course with Instructor
                var course = await courseRepo.GetAsyncSpec(new CourseWithInstructorSpec(id, userId));
                if (course is null) return ApplicationServiceResult<CourseResponseForInstructor>.Fail(errorMessage);

                // Update Course Mapping
                _mapper.Map(req, course);

                courseRepo.Update(course);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<CourseResponseForInstructor>(course);
                return ApplicationServiceResult<CourseResponseForInstructor>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<CourseResponseForInstructor>.Fail(loggerError);
            }
        }
        #endregion

        #region Delete Course Async
        public async Task<ApplicationServiceResult<bool>> DeleteCourseAsync(int id)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No course Found with this id";
            var succeededMessage = "Course Deleted succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<bool>
                        .Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                // Get Course
                var course = await courseRepo.GetAsyncSpec(new CourseWithInstructorSpec(id, userId));
                if (course is null) return ApplicationServiceResult<bool>.Fail(errorMessage);

                course.IsDeleted = true;
                courseRepo.Update(course);
                await _unitOfWork.CompleteAsync();
                return ApplicationServiceResult<bool>.Success(true, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<bool>.Fail(loggerError);
            }
        }
        #endregion

        #region Delete Courses Async
        public async Task<ApplicationServiceResult<DeleteCoursesResult>> DeleteCoursesAsync(IEnumerable<int> courseIds)
        {
            var userNotFoundMessage = "User Not Found";
            var succeededMessage = "Course Deleted succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<DeleteCoursesResult>
                    .Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                // All Courses For Specification Instructors
                var courses = await courseRepo.GetAllAsyncSpec(new CourseWithInstructorSpec(courseIds, userId));

                // Get All Courses With Ids
                var allCourses = await courseRepo.GetAllAsyncSpec(new CourseWithInstructorSpec(courseIds));

                // Existing Courses
                var existingIds = courses.Select(c => c.Id).ToList();
                // All Courses with Ids Request [authorization - unauthorized]
                var allIds = allCourses.Select(c => c.Id).ToList();

                var notFoundIds = courseIds.Except(allIds).ToList();    // Ids not found in database
                var unauthorizedIds = allIds.Except(existingIds).ToList(); // Ids not for this instructor

                foreach (var course in courses)
                {
                    courseRepo.Delete(course);
                }

                await _unitOfWork.CompleteAsync();
                var result = new DeleteCoursesResult
                {
                    DeletedIds = existingIds,
                    NotFoundIds = notFoundIds,
                    UnauthorizedIds = unauthorizedIds
                };

                return ApplicationServiceResult<DeleteCoursesResult>.Success(result, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<DeleteCoursesResult>.Fail(loggerError);
            }
        }
        #endregion
    }
}

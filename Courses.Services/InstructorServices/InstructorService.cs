using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Students;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Instructors;
using Courses.Core.ModelsDTO.ResponseDTO.Students;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.EnrollmentSpecifications;
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
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly IMapper _mapper;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger, ICurrentUserService currentUserService, IMapper mapper, ICurrentInstructorServices currentInstructorServices)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _currentInstructorServices = currentInstructorServices;
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
        public async Task<ApplicationServiceResult<Pagination<CourseResponseForInstructor>>> GetAllCoursesAsync(CoursesParams @params)
        {
            var userNotFoundMessage = "User Not Found";
            var errorMessage = "No Courses For this Instructor";
            var succeededMessage = "this all courses";
            var loggerError = "There is a problem in database";

            try
            {
                var userId = _currentUserService.UserId;
                if (userId is null) return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(userNotFoundMessage);

                var courseRepo = _unitOfWork.CreateRepository<Course>();
                var spec = new CourseWithInstructorSpec(userId, @params);
                var courses = await courseRepo.GetAllAsyncSpec(spec);
                var courseCount = await courseRepo.GetCountAsyncSpec(new CoursesCountWithSpec(@params));
                if (!courses.Any()) return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(errorMessage);

                var dataMapping = _mapper.Map<IReadOnlyList<CourseResponseForInstructor>>(courses);
                var data = new Pagination<CourseResponseForInstructor>(
                        @params.PageIndex,
                        @params.PageSize,
                        courseCount,
                        dataMapping
                    );

                return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ApplicationServiceResult<Pagination<CourseResponseForInstructor>>.Fail(loggerError);
            }
        }

        #endregion

        #region Get Students Instructor Async
        /// <summary>
        /// Two-phase query approach to correctly paginate and count at the (STUDENT level)
        /// (not the enrollment level), and to ensure CourseCount reflects ALL courses
        /// a student is enrolled in with this instructor (not just search-matching ones).
        ///
        /// Phase 1: Find matching student IDs (with search filter, no pagination, lightweight — no Course include).
        /// Phase 2: Fetch full enrollment data for the paginated students (with Course include, no search filter).
        /// </summary>
        public async Task<ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>> GetStudentsInstructorAsync(StudentParams @params)
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var succeededMessage = "this all students for instructor";
            var loggerError = "There is a problem in database";
            int? instructorId = null;
            try
            {
                instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>.Fail(userNotFoundMessage);

                var enrollmentRepo = _unitOfWork.CreateRepository<Enrollment>();

                // ── Phase 1: Find matching student IDs ──────────────────────────
                // Lightweight query: includes Student (needed for name search & sorting)
                var searchSpec = new EnrollmentWithStudentSpec(
                    instructorId, @params, forCount: false, applyPagination: false, includeCourse: false);
                var searchEnrollments = await enrollmentRepo.GetAllAsyncSpec(searchSpec);

                // Empty result is a valid state — return Success with empty pagination
                if (!searchEnrollments.Any())
                    return ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>.Success(
                        new Pagination<StudentWithInstructorResponse>(
                            @params.PageIndex, @params.PageSize, 0,
                            new List<StudentWithInstructorResponse>().AsReadOnly()),
                        succeededMessage);

                // Group by student to get distinct student IDs with sort key values
                var studentGroups = searchEnrollments
                    .GroupBy(x => new { x.StudentId, x.Student.Name })
                    .Select(group => new StudentSearchInfo(
                        group.Key.StudentId,
                        group.Key.Name,
                        group.Min(x => x.CreatedAt)
                    )).ToList();

                // Apply in-memory sorting at the STUDENT level
                studentGroups = ApplyStudentSorting(studentGroups, @params);

                // Total count of distinct matching students
                var totalStudentsCount = studentGroups.Count;

                // Apply in-memory pagination at the STUDENT level
                var paginatedStudentIds = studentGroups
                    .Skip(@params.PageSize * (@params.PageIndex - 1))
                    .Take(@params.PageSize)
                    .Select(s => s.StudentId)
                    .ToList();

                // Page beyond available data — return Success with empty pagination
                if (!paginatedStudentIds.Any())
                    return ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>.Success(
                        new Pagination<StudentWithInstructorResponse>(
                            @params.PageIndex, @params.PageSize, totalStudentsCount,
                            new List<StudentWithInstructorResponse>().AsReadOnly()),
                        succeededMessage);

                // ── Phase 2: Fetch full data for paginated students ──────────────
                // No search filter — retrieves ALL courses for these students with
                // this instructor, so CourseCount and Courses list are accurate.
                var dataSpec = new EnrollmentWithStudentSpec(instructorId, paginatedStudentIds);
                var dataEnrollments = await enrollmentRepo.GetAllAsyncSpec(dataSpec);

                // Group by student and map to response DTOs
                var students = dataEnrollments
                    .GroupBy(x => new { x.StudentId, x.Student.Name })
                    .Select(group => new StudentWithInstructorResponse
                    {
                        Id = group.Key.StudentId,
                        Name = group.Key.Name,
                        CourseCount = group.Select(x => x.CourseId).Distinct().Count(),
                        FirstEnrollment = group.Min(x => x.CreatedAt),
                        Courses = group.Select(x => new StudentCourseWithInstructor
                        {
                            Id = x.CourseId,
                            Name = x.Course.Name,
                            EnrollmentAt = x.CreatedAt
                        }).ToList()
                    }).ToList();

                // Preserve the sorted order from Phase 1 by ordering according to paginatedStudentIds
                var studentsDict = students.ToDictionary(s => s.Id);
                students = paginatedStudentIds
                    .Where(id => studentsDict.ContainsKey(id))
                    .Select(id => studentsDict[id])
                    .ToList();

                var dataMapping = new Pagination<StudentWithInstructorResponse>(
                        pageIndex: @params.PageIndex,
                        pageSize: @params.PageSize,
                        count: totalStudentsCount,
                        data: students
                    );

                return ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>.Success(dataMapping, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There is a problem in database when try to retrieve Student for instructor {instructorId}", instructorId);
                return ApplicationServiceResult<Pagination<StudentWithInstructorResponse>>.Fail(loggerError);
            }
        }
        #endregion

        #region Get Student Instructor Async
        public async Task<ApplicationServiceResult<StudentWithInstructorResponse>> GetStudentInstructorAsync(int id)
        {
            var userNotFoundMessage = "There is no instructor with this id";
            var errorMessage = "There is No Student";
            var succeededMessage = "you retrieve student details succeeded";
            var loggerError = "There is a problem in database";

            try
            {
                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null)
                    return ApplicationServiceResult<StudentWithInstructorResponse>.Fail(userNotFoundMessage);

                var spec = new EnrollmentWithStudentSpec(instructorId, id);
                // Get All Enrollments For One Student
                var enrollments = await _unitOfWork.CreateRepository<Enrollment>().GetAllAsyncSpec(spec);
                
                if (!enrollments.Any())
                    return ApplicationServiceResult<StudentWithInstructorResponse>.Fail(errorMessage);

                // For Get ID And Name
                var firstEnrollment = enrollments.First();
                var student = new StudentWithInstructorResponse
                {
                    Id = firstEnrollment.Student.Id,
                    Name = firstEnrollment.Student.Name,
                    CourseCount = enrollments.Select(x => x.CourseId).Distinct().Count(),
                    FirstEnrollment = enrollments.Min(x => x.CreatedAt),
                    Courses = enrollments
                    .Select(x => new StudentCourseWithInstructor
                    {
                        Id = x.Course.Id,
                        Name = x.Course.Name,
                        EnrollmentAt = x.CreatedAt
                    }).ToList(),
                };
                return ApplicationServiceResult<StudentWithInstructorResponse>.Success(student, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Student details id {id}", id);
                return ApplicationServiceResult<StudentWithInstructorResponse>.Fail(loggerError);
            }
        }
        #endregion

        #region Helper Methods
        private async Task<int?> GetCurrentInstructorInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            return instructorInfo.Data?.Id;
        }

        /// <summary>
        /// In-memory sorting at the STUDENT level (after grouping enrollments by student).
        /// "courseName" sorts are ambiguous at student level (a student has multiple courses),
        /// so they fall back to name sorting.
        /// </summary>
        private List<StudentSearchInfo> ApplyStudentSorting(List<StudentSearchInfo> students, StudentParams @params)
        {
            switch (@params.Sort?.ToLower())
            {
                case "namedesc":
                    return students.OrderByDescending(s => s.StudentName).ToList();

                case "firstenrollment":
                    return students.OrderBy(s => s.FirstEnrollment).ToList();

                case "firstenrollmentdesc":
                    return students.OrderByDescending(s => s.FirstEnrollment).ToList();

                default:
                    return students.OrderBy(s => s.StudentName).ToList();
            }
        }

        /// <summary>
        /// Lightweight struct to hold Phase 1 search results for in-memory
        /// student-level sorting and pagination before Phase 2 data fetch.
        /// </summary>
        private record StudentSearchInfo(int StudentId, string StudentName, DateTime FirstEnrollment);
        #endregion
    }
}

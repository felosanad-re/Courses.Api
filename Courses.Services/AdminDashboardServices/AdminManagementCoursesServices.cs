using AutoMapper;
using AutoMapper.QueryableExtensions;
using Courses.Core.GenericRepository;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.AdminDashboardServices;
using Courses.Core.Services.Contract.UserServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Courses.Services.AdminDashboardServices
{
    public class AdminManagementCoursesServices : IAdminManagementCoursesServices
    {
        #region Services
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<AdminManagementCoursesServices> _logger;
        protected readonly IConfiguration _configuration;

        public AdminManagementCoursesServices(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, IMapper mapper, ILogger<AdminManagementCoursesServices> logger, IConfiguration configuration)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Get All Courses Async
        public async Task<ApplicationServiceResult<Pagination<AdminCoursesResponse>>> GetAllCoursesAsync(CoursesParams param, CourseType courseType)
        {
            const string SucceededMessage = "You get all courses succeeded.";
            const string WarningMessage = "There is no courses yet.";
            const string LoggerMessage = "Failed to retrieve Courses.";
            string? userId = _currentUserService.UserId;

            try
            {
                // Get All Courses
                var couresesSpec = new CoursesWithSpec(param, courseType);
                var coursesCountSpec = new CoursesCountWithSpec(param);
                var coursesRepo = _unitOfWork.CreateRepository<Course>();

                var totalCoursesCount = await coursesRepo.GetCountAsyncSpec(coursesCountSpec);
                if (totalCoursesCount == 0)
                    return ApplicationServiceResult<Pagination<AdminCoursesResponse>>.Success(new Pagination<AdminCoursesResponse>(param.PageIndex, param.PageSize, 0, new List<AdminCoursesResponse>()), WarningMessage);

                var courses = await coursesRepo.GetQuerySpec(couresesSpec)
                    .ProjectTo<AdminCoursesResponse>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // Get Full Path For Images
                foreach (var course in courses)
                {
                    course.Image = $"{_configuration["BasePictureUrl"]}/Files/Images/{course.Image}";
                }

                var pagination = new Pagination<AdminCoursesResponse>(
                        param.PageIndex,
                        param.PageSize,
                        totalCoursesCount,
                        courses
                    );

                return ApplicationServiceResult<Pagination<AdminCoursesResponse>>.Success(pagination, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Courses for user Id {userId}", userId);
                return ApplicationServiceResult<Pagination<AdminCoursesResponse>>.Success(new Pagination<AdminCoursesResponse>(param.PageIndex, param.PageSize, 0, new List<AdminCoursesResponse>()), LoggerMessage);
            }
        }
        #endregion

        #region Get Course Details Async
        public async Task<ApplicationServiceResult<CourseDetailsToReturnDTO>> GetCourseDetailsAsync(int courseId, CourseType type)
        {
            const string SucceededMessage = "You get course details succeeded.";
            const string ErrorMessage = "There is no course with this Id.";
            const string LoggerMessage = "Failed to retrieve Course details.";
            string? userId = _currentUserService.UserId;

            // Get All Courses
            try
            {
                var coureseSpec = new CoursesWithSpec(courseId, type);
                var courseRepo = _unitOfWork.CreateRepository<Course>();

                CourseDetailsToReturnDTO? course;

                if(type == CourseType.RecorderCourse)
                {
                    course = await GetRecordedCourseDetails(courseRepo, coureseSpec);
                    course.Image = $"{_configuration["BasePictureUrl"]}/Files/Images/{course.Image}";
                }
                else
                {
                    course = await GetOnlineCourseDetails(courseRepo, coureseSpec);
                    course.Image = $"{_configuration["BasePictureUrl"]}/Files/Images/{course.Image}";
                }

                if(course is null)
                    return ApplicationServiceResult<CourseDetailsToReturnDTO>.Fail(ErrorMessage);
                return ApplicationServiceResult<CourseDetailsToReturnDTO>.Success(course, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to retrieve Course details for user Id {userId}", userId);
                return ApplicationServiceResult<CourseDetailsToReturnDTO>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Update Course Status Async
        public async Task<ApplicationServiceResult<bool>> UpdateCourseStatusAsync(int courseId, UpdateCourseStatusRequest req)
        {
            const string SucceededMessage = "Course status updated successfully.";
            const string WarningMessage = "The course already has the same status.";
            const string ErrorMessage = "There is no course with this ID.";
            const string LoggerMessage = "Failed to update course status.";
            string? userId = _currentUserService.UserId;

            try
            {
                var courseSpec = new CoursesWithoutIncludesSpec(courseId);
                var courseRepo = _unitOfWork.CreateRepository<Course>();

                var course = await courseRepo.GetAsyncSpec(courseSpec);
                if (course is null)
                    return ApplicationServiceResult<bool>.Fail(ErrorMessage);

                if (course.Status == req.Status)
                    return ApplicationServiceResult<bool>.Fail(WarningMessage);

                course.Status = req.Status;
                var res = await _unitOfWork.CompleteAsync();
                if (res <= 0)
                    return ApplicationServiceResult<bool>.Fail("Failed to update course status");

                return ApplicationServiceResult<bool>.Success(true, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to update course status {courseId} for user {userId}", courseId, userId);
                return ApplicationServiceResult<bool>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Delete Course Async
        public async Task<ApplicationServiceResult<bool>> DeleteCourseAsync(int courseId)
        {
            const string SucceededMessage = "Course deleted successfully.";
            const string WarningMessage = "The course already Deleted.";
            const string ErrorMessage = "There is no course with this ID.";
            const string LoggerMessage = "Failed to update course status.";
            string? userId = _currentUserService.UserId;

            try
            {
                var courseSpec = new CoursesWithoutIncludesSpec(courseId);
                var courseRepo = _unitOfWork.CreateRepository<Course>();

                var course = await courseRepo.GetAsyncSpec(courseSpec);
                if (course is null)
                    return ApplicationServiceResult<bool>.Fail(ErrorMessage);

                if (course.IsDeleted)
                    return ApplicationServiceResult<bool>.Fail(WarningMessage);

                course.IsDeleted = true;
                var res = await _unitOfWork.CompleteAsync();
                if (res <= 0)
                    return ApplicationServiceResult<bool>.Fail("Failed to deleted course");

                return ApplicationServiceResult<bool>.Success(true, SucceededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to update course status {courseId} for user {userId}", courseId, userId);
                return ApplicationServiceResult<bool>.Fail(LoggerMessage);
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// method to get Recorded course details
        /// </summary>
        /// <param name="courseRepo"></param>
        /// <param name="courseSpec"></param>
        /// <returns>CourseDetailsToReturnDTO => Recorded Course Details</returns>
        private async Task<CourseDetailsToReturnDTO?> GetRecordedCourseDetails(IGenericRepository<Course> courseRepo, BaseSpecifications<Course> courseSpec)
        {
            return await courseRepo.GetQuerySpec(courseSpec)
                .Select(c => new CourseDetailsToReturnDTO
                {
                    CourseCategory = c.CourseCategory.Name,
                    Name = c.Name,
                    Id = c.Id,
                    Description = c.Description,
                    CourseCategoryId = c.CourseCategoryId,
                    InstructorId = c.InstructorId,
                    InstructorName = c.Instructor.Name,
                    Price = c.Price,
                    Image = c.Image,
                    IsPaid = c.IsPaid,
                    Status = c.Status.ToString(),
                    Type = c.Type.ToString(),
                    Sections = c.Sections.Select(s => new SectionToReturnDTO
                    {
                        CourseId = s.CourseId,
                        Id = s.Id,
                        CourseName = s.Course.Name,
                        Title = s.Title,
                        Content = s.Lectures.Select(l => new CourseContentItemDTO
                        {
                            Id = l.Id,
                            SectionId = l.SectionId,
                            SectionName = l.Section.Title,
                            Title = l.Title,
                            Url = l.VideoUrl,
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// method to get Online course details
        /// </summary>
        /// <param name="courseRepo"></param>
        /// <param name="courseSpec"></param>
        /// <returns>CourseDetailsToReturnDTO => Online Course Details</returns>
        private async Task<CourseDetailsToReturnDTO?> GetOnlineCourseDetails(IGenericRepository<Course> courseRepo, BaseSpecifications<Course> courseSpec)
        {
            return await courseRepo.GetQuerySpec(courseSpec)
            .Select(c => new CourseDetailsToReturnDTO
            {
                CourseCategory = c.CourseCategory.Name,
                Name = c.Name,
                Id = c.Id,
                Description = c.Description,
                CourseCategoryId = c.CourseCategoryId,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor.Name,
                Price = c.Price,
                Image = c.Image,
                IsPaid = c.IsPaid,
                Status = c.Status.ToString(),
                Type = c.Type.ToString(),
                Sections = c.Sections.Select(s => new SectionToReturnDTO
                {
                    CourseId = s.CourseId,
                    Id = s.Id,
                    CourseName = s.Course.Name,
                    Title = s.Title,
                    Content = s.Sessions.Select(ss => new CourseContentItemDTO
                    {
                        Id = ss.Id,
                        SectionId = ss.SectionId,
                        SectionName = ss.Section.Title,
                        Title = ss.Topic,
                        Url = ss.StudentJoinUrl,
                    }).ToList()
                }).ToList()
            }).FirstOrDefaultAsync();
        }
        #endregion
    }
}

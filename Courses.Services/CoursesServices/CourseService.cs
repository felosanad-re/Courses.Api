using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Courses;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.CoursesServices;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.CoursesServices
{
    public class CourseService : ICourseService
    {
        #region DI
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<CourseService> _logger;
        public CourseService(IUnitOfWork unitOfWork, ILogger<CourseService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region Get All Courses
        public async Task<ApplicationServiceResult<Pagination<CoursesToReturnDTO>>> GetAllCoursesAsync(CoursesParams @params)
        {
            var errorMessage = "there is no courses yet";
            var succeeddedMessage = "this all courses";
            var logError = "there is a problem in database";

            try
            {
                var coursesRepo = _unitOfWork.CreateRepository<Course>();
                var courses = await coursesRepo.GetAllAsyncSpec(new CoursesWithSpec(@params));
                var coursesCount = await coursesRepo.GetCountAsyncSpec(new CoursesCountWithSpec(@params));
                if (coursesCount == 0) return ApplicationServiceResult<Pagination<CoursesToReturnDTO>>.Fail(errorMessage);

                var pagData = new Pagination<CoursesToReturnDTO>
                    (
                        @params.PageIndex,
                        @params.PageSize,
                        coursesCount,
                        _mapper.Map<IReadOnlyList<CoursesToReturnDTO>>(courses)
                    );

                return ApplicationServiceResult<Pagination<CoursesToReturnDTO>>.Success(pagData, succeeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all courses with params {@Params}", @params);
                return ApplicationServiceResult<Pagination<CoursesToReturnDTO>>.Fail(logError);
            }
        }
        #endregion

        #region Get Course Details
        public async Task<ApplicationServiceResult<CourseDetailsToReturnDTO>> GetCourseDetailsAsync(int courseId, CourseType type)
        {
            var errorMessage = "there is no course with this id";
            var succeeddedMessage = "this is a course details";
            var logError = "there is a problem in database";

            try
            {

                BaseSpecifications<Course> spec = type switch
                {
                    CourseType.OnlineCourse => new OnlineCoursesWithSpec(courseId),
                    CourseType.RecorderCourse => new RecordedCoursesWithSpec(courseId),
                    _ => throw new ArgumentOutOfRangeException(nameof(type))
                };

                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(spec);
                if (course is null) return ApplicationServiceResult<CourseDetailsToReturnDTO>.Fail(errorMessage);

                var data = _mapper.Map<CourseDetailsToReturnDTO>(course);
                data.Sections = course.Sections
                    .OrderBy(section => section.Order)
                    .Select(section =>
                    {
                        var sectionDto = _mapper.Map<SectionToReturnDTO>(section);

                        var lectures = section.Lectures
                            .Select(lecture => new
                            {
                                lecture.Order,
                                Content = _mapper.Map<CourseContentItemDTO>(lecture)
                            });

                        var sessions = section.Sessions
                            .Select(session => new
                            {
                                session.Order,
                                Content = _mapper.Map<CourseContentItemDTO>(session)
                            });

                        sectionDto.Content = lectures
                            .Concat(sessions)
                            .OrderBy(item => item.Order)
                            .Select(item => item.Content)
                            .ToList();

                        return sectionDto;
                    })
                    .ToList();

                return ApplicationServiceResult<CourseDetailsToReturnDTO>.Success(data, succeeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting the course with this Id: {courseId}", courseId);
                return ApplicationServiceResult<CourseDetailsToReturnDTO>.Fail(logError);
            }
        }

        #endregion
    }
}

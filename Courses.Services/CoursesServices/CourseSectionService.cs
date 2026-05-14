using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.CoursesServices;
using Courses.Core.Specifications.SectionsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.CoursesServices
{
    public class CourseSectionService : ICourseSectionService
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<CourseSectionService> _logger;
        public CourseSectionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CourseSectionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>> GetAllSections(int courseId)
        {
            var errorMessage = "there is no sections with this course";
            var succeededMessage = "this all sections for this course";
            var loggerError = "there is a problem in database";


            try
            {
                var sections = await _unitOfWork.CreateRepository<Section>().GetAllAsyncSpec(new SectionWithSpec(courseId));
                if (!sections.Any()) return ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>.Fail(errorMessage);

                var data = _mapper.Map<IReadOnlyList<SectionWithCourseResponse>>(sections);

                return ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there Error occurred while getting the Sections with this CourseId: {courseId}", courseId);
                return ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>.Fail(loggerError);
            }
        }
    }
}

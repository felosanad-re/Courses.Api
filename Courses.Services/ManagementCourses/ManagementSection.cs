using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.Specifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ManagementCourses
{
    public class ManagementSection : IManagementSection
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly IMapper _mapper;
        protected readonly ILogger<ManagementSection> _logger;

        public ManagementSection(IUnitOfWork unitOfWork, IMapper mapper, ICurrentInstructorServices currentInstructorServices, ILogger<ManagementSection> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentInstructorServices = currentInstructorServices;
            _logger = logger;
        }
        #endregion

        #region Create Section Async
        public async Task<ApplicationServiceResult<SectionWithCourseResponse>> CreateSectionAsync(CreateSectionRequest req)
        {
            var loggerMessage = "there is a problem in database";
            try
            {
                var errorMessage = "you don't have access for this course";
                var succeededMessage = "Section Created Succeedded In database";

                // Get Current Instructor
                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null) return ApplicationServiceResult<SectionWithCourseResponse>.Fail("there    is no instructor with this id");

                    // Get Course for this section
                    var sectionWithCourseSpec = new BaseSpecifications<Course>(
                        x => (x.Id == req.CourseId) &&
                             (x.InstructorId == instructorId)
                        );

                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(sectionWithCourseSpec);
                if (course is null)
                    return ApplicationServiceResult<SectionWithCourseResponse>.Fail("Course not found or you don't have access");

                // Create new Section
                var section = _mapper.Map<Section>(req); // Create New Section
                await _unitOfWork.CreateRepository<Section>().AddAsync(section);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<SectionWithCourseResponse>(section);

                return ApplicationServiceResult<SectionWithCourseResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to create new section in courseId: {CourseId}", req.CourseId);
                return ApplicationServiceResult<SectionWithCourseResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Update Section Async
        public async Task<ApplicationServiceResult<SectionWithCourseResponse>> UpdateSectionAsync(UpdateSectionRequest req)
        {
            var loggerMessage = "there is a problem in database";
            Section? section = null;
            try
            {
                var errorMessage = "Section not found or access denied";
                var succeededMessage = "Section Updated Succeedded In database";

                // Get Current Instructor
                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null) return ApplicationServiceResult<SectionWithCourseResponse>.Fail("there is no instructor with this id");

                // Get Section With has Own Course
                var sectionWithCourseSpec = new BaseSpecifications<Section>
                    (
                        x => (x.Id == req.Id) &&
                             (x.Course.InstructorId == instructorId)
                    );

                sectionWithCourseSpec.Includes.Add(x => x.Course);

                section = await _unitOfWork.CreateRepository<Section>()
                    .GetAsyncSpec(sectionWithCourseSpec);

                if (section == null) return ApplicationServiceResult<SectionWithCourseResponse>.Fail(errorMessage);
                _mapper.Map(req, section);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<SectionWithCourseResponse>(section);

                return ApplicationServiceResult<SectionWithCourseResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to update SectionId: {Id}", section?.Id);
                return ApplicationServiceResult<SectionWithCourseResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Delete Section Async
        public Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteSectionAsync(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region
        public Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteMultiSections(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Helper method
        private async Task<int?> GetCurrentInstructorInfo()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if (instructorInfo is null || !instructorInfo.Succeed || instructorInfo.Data is null)
                return null;

            return instructorInfo.Data.Id;
        }
        #endregion
    }
}

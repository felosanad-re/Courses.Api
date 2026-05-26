using AutoMapper;
using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.Specifications;
using Courses.Core.Specifications.CoursesSpecifications;
using Courses.Core.Specifications.SectionsSpecifications;
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
                var errorMessage = "Course not found or you don't have access";
                var succeededMessage = "Section Created Succeedded In database";

                // Get Current Instructor
                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null) return ApplicationServiceResult<SectionWithCourseResponse>.Fail("there    is no instructor with this id");

                // Get Course for this section
                var sectionWithCourseSpec = new CoursesWithSectionsSpec(req.CourseId, instructorId);

                var course = await _unitOfWork.CreateRepository<Course>().GetAsyncSpec(sectionWithCourseSpec);

                if (course is null)
                    return ApplicationServiceResult<SectionWithCourseResponse>.Fail(errorMessage);

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
                var sectionWithCourseSpec = new SectionWithCourseSpec(req.Id, instructorId);

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
        public async Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteSectionAsync(int id)
        {
            var loggerMessage = "there is a problem in database";
            try
            {
                var errorMessage = "Section not found or access denied";
                var succeededMessage = "Section Deleted Succeeded From database";

                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null) return ApplicationServiceResult<DeleteSectionResponse>.Fail("there is no instructor with this id");

                // Get Section
                var spec = new SectionWithCourseSpec(id, instructorId);

                var section = await _unitOfWork.CreateRepository<Section>().GetAsyncSpec(spec);
                if (section is null) return ApplicationServiceResult<DeleteSectionResponse>.Fail(errorMessage);

                // Get Lectures Count Before Delete Section 
                var lecturesCount = section.Lectures.Count;

                _unitOfWork.CreateRepository<Section>().Delete(section);
                await _unitOfWork.CompleteAsync();

                var data = new DeleteSectionResponse()
                {
                    Message = "you delete one section",
                    LecturesCount = lecturesCount,
                    SectionsCount = 1
                };

                return ApplicationServiceResult<DeleteSectionResponse>.Success(data, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to delete sectionId: {id}", id);
                return ApplicationServiceResult<DeleteSectionResponse>.Fail(loggerMessage);
            }

        }
        #endregion

        #region Delete Multi Sections
        public async Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteMultiSections(IEnumerable<int> ids)
        {
            var loggerMessage = "there is a problem in database";
            try
            {
                var errorMessage = "Section not found or access denied";
                var succeededMessage = "Sections Deleted Succeeded From database";

                var instructorId = await GetCurrentInstructorInfo();
                if (instructorId is null) return ApplicationServiceResult<DeleteSectionResponse>.Fail("there is no instructor with this id");

                var spec = new SectionWithCourseSpec(ids, instructorId);

                var sections = await _unitOfWork.CreateRepository<Section>().GetAllAsyncSpec(spec);
                if (!sections.Any()) return ApplicationServiceResult<DeleteSectionResponse>.Fail(errorMessage);
                // Get Num Of Sections
                var sectionsCount = sections.Count;
                // Get Num Of Lecturers
                var lecturesCount = sections.Sum(x => x.Lectures.Count);

                // Delete Sections And Lectures
                foreach (var section in sections)
                {
                    section.IsDeleted = true;
                    foreach (var lecture in section.Lectures)
                    {
                        lecture.IsDeleted = true;
                    }
                }

                await _unitOfWork.CompleteAsync();
                var result = new DeleteSectionResponse()
                {
                    Message = "Deleted Section Succeeded",
                    SectionsCount = sectionsCount,
                    LecturesCount = lecturesCount,
                };
                return ApplicationServiceResult<DeleteSectionResponse>.Success(result, succeededMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to delete sectionId: {ids}", ids);
                return ApplicationServiceResult<DeleteSectionResponse>.Fail(loggerMessage);
            }
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

using AutoMapper;
using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Lectures;
using Courses.Core.ModelsDTO.ResponseDTO.Lectures;
using Courses.Core.Services.Contract.InstructorServices;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.Specifications;
using Courses.Core.Specifications.LectureSpecifications;
using Courses.Core.Specifications.SectionsSpecifications;
using Courses.Core.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Courses.Services.ManagementCourses
{
    public class ManagementLecture : IManagementLecture
    {
        #region DI Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly ICurrentInstructorServices _currentInstructorServices;
        protected readonly ILogger<ManagementLecture> _logger;
        protected readonly IMapper _mapper;

        public ManagementLecture(IUnitOfWork unitOfWork, ILogger<ManagementLecture> logger, IMapper mapper, ICurrentInstructorServices currentInstructorServices)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _currentInstructorServices = currentInstructorServices;
        }
        #endregion

        #region Create Lecture Async
        public async Task<ApplicationServiceResult<LectureWithInstructorResponse>> CreateLectureAsync(CreatedLectureRequest req)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no section or u don't have access for this section";
                var succeddedMessage = "The lecture Added Succeeded";

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(instructorError);

                // Get Section
                var spec = new SectionWithCourseSpec(req.SectionId, instructorId);

                var section = await _unitOfWork.CreateRepository<Section>().GetAsyncSpec(spec);
                if (section is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                var lecture = _mapper.Map<Lecture>(req);

                await _unitOfWork.CreateRepository<Lecture>().AddAsync(lecture);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<LectureWithInstructorResponse>(lecture);

                return ApplicationServiceResult<LectureWithInstructorResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to add new lecture in section id {id}", req.SectionId);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Update Lecture Async
        public async Task<ApplicationServiceResult<LectureWithInstructorResponse>> UpdateLectureAsync(UpdatedLectureRequest req)
        {
            var loggerMessage = "there is a problem in database";

            try
            {
                var instructorError = "There is no instructor with this id";
                var errorMessage = "there is no lecture or u don't have access for this lecture";
                var succeddedMessage = "The lecture updated Succeeded";

                var instructorId = await GetCurrentInstructor();
                if (instructorId is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(instructorError);

                var spec = new LectureWithInstructorSpec(req.Id, instructorId);

                var lecture = await _unitOfWork.CreateRepository<Lecture>().GetAsyncSpec(spec);
                if (lecture is null) return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(errorMessage);

                _mapper.Map(req, lecture);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<LectureWithInstructorResponse>(lecture);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Success(data, succeddedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "there is a problem when try to updated lecture in section id {id}", req.SectionId);
                return ApplicationServiceResult<LectureWithInstructorResponse>.Fail(loggerMessage);
            }
        }
        #endregion

        #region Delete Lecture Async
        public Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteLectureAsync(int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Delete Multi Lecture Async
        public Task<ApplicationServiceResult<LectureDeletedResponse>> DeleteMultiLectureAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helper Method
        public async Task<int?> GetCurrentInstructor()
        {
            var instructorInfo = await _currentInstructorServices.GetCurrentInstructor();
            if(instructorInfo is null) return null;

            return instructorInfo.Data?.Id;
        }
        #endregion
    }
}

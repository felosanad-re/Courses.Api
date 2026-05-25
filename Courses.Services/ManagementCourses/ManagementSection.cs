using AutoMapper;
using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;
using Courses.Core.Services.Contract.ManagementCourses;
using Courses.Core.UnitOfWork;

namespace Courses.Services.ManagementCourses
{
    public class ManagementSection : IManagementSection
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        public ManagementSection(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public Task<ApplicationServiceResult<SectionWithCourseResponse>> CreateSectionAsync(CreateSectionRequest req)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationServiceResult<SectionWithCourseResponse>> UpdateSectionAsync(UpdateSectionRequest req)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteMultiSections(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteSectionAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

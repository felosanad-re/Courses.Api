using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.RequestDTO.Sections;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;

namespace Courses.Core.Services.Contract.ManagementCourses
{
    public interface IManagementSection
    {
        // Create Sections After Instructor Create Course
        Task<ApplicationServiceResult<SectionWithCourseResponse>> CreateSectionAsync(CreateSectionRequest req);

        // Update Section
        Task<ApplicationServiceResult<SectionWithCourseResponse>> UpdateSectionAsync(UpdateSectionRequest req);

        // Delete Section With Lectures
        Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteSectionAsync(int id);

        // Delete Multi-Sections
        Task<ApplicationServiceResult<DeleteSectionResponse>> DeleteMultiSections(IEnumerable<int> ids);
    }
}

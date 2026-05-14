using Courses.Core.ModelsDTO;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;

namespace Courses.Core.Services.Contract.CoursesServices
{
    public interface ICourseSectionService
    {
        // Get All Sections For Course [Lectures(Name - Id) - SectionOrder - Title]
        Task<ApplicationServiceResult<IReadOnlyList<SectionWithCourseResponse>>> GetAllSections(int courseId);
    }
}

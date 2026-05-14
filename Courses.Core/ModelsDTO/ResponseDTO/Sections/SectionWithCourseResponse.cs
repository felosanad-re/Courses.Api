using Courses.Core.ModelsDTO.ResponseDTO.Lectures;

namespace Courses.Core.ModelsDTO.ResponseDTO.Sections
{
    public class SectionWithCourseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }

        public List<LectureWithSectionResponse> Lectures { get; set; } = new();
    }
}

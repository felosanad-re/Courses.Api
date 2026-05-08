using Courses.Core.ModelsDTO.ResponseDTO.Lectures;

namespace Courses.Core.ModelsDTO.ResponseDTO.Sections
{
    public class SectionResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }


        public string CourseName { get; set; } // NFP
        public int CourseId { get; set; }

        public ICollection<LectureResponse> Lectures { get; set; }
    }
}

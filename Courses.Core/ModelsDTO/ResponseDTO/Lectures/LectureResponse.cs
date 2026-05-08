using Courses.Core.ModelsDTO.ResponseDTO.StudentLectureProgress;

namespace Courses.Core.ModelsDTO.ResponseDTO.Lectures
{
    public class LectureResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; }


        // The section this lecture belongs to (many-to-one)
        public string SectionName { get; set; } //NFP
        public int SectionId { get; set; }

        // Tracks which students have completed this lecture
        public List<StudentLectureProgressResponse> StudentProgresses { get; set; }
    }
}

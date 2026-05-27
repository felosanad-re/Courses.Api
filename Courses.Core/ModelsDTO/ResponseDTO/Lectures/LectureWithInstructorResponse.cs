namespace Courses.Core.ModelsDTO.ResponseDTO.Lectures
{
    public class LectureWithInstructorResponse
    {
        public string Title { get; set; }

        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; }

        // To Get Lecture newt or Previous
        public int Order { get; set; }
        // Duration for video
        public int DurationInSeconds { get; set; }

        // For Paid lecture --> can't Preview
        public bool IsPreview { get; set; }

        // The section this lecture belongs to (many-to-one)
        public string SectionName { get; set; }
        public int SectionId { get; set; }
    }
}

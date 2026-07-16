namespace Courses.Core.ModelsDTO.ResponseDTO.Lectures
{
    /// <summary>
    /// This Response course content if course id Recorded or online
    /// </summary>
    public class CourseContentItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }

        // URL to the lecture video (could be a streaming URL or file path)
        public string Url { get; set; }


        // The section this lecture belongs to (many-to-one)
        public string SectionName { get; set; } //NFP
        public int SectionId { get; set; }
    }
}

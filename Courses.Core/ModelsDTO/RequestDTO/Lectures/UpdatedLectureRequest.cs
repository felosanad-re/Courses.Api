using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Lectures
{
    public class UpdatedLectureRequest
    {
        [Required]
        public int Id { get; set; }
        public string Title { get; set; }
        [Url]

        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; }

        // To Get Lecture newt or Previous
        public int Order { get; set; }

        // Duration for video
        public int DurationInSeconds { get; set; }

        // For Paid lecture --> can't Preview
        public bool IsPreview { get; set; }

        [Required]
        public int SectionId { get; set; }
    }
}

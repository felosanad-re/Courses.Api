using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Lectures
{
    public class CreatedLectureRequest
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Url]
        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; }

        [Required]
        // To Get Lecture newt or Previous
        public int Order { get; set; }
        [Required]
        // Duration for video
        public int DurationInSeconds { get; set; }
        [Required]
        // For Paid lecture --> can't Preview
        public bool IsPreview { get; set; }
        [Required]
        public int SectionId { get; set; }
    }
}

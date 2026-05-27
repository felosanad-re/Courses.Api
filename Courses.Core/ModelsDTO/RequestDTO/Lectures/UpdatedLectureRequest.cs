using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Lectures
{
    public class UpdatedLectureRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Url]
        [MaxLength(1000)]

        // URL to the lecture video (could be a streaming URL or file path)
        public string VideoUrl { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        // To Get Lecture newt or Previous
        public int Order { get; set; }

        [Range(1, int.MaxValue)]
        // Duration for video
        public int DurationInSeconds { get; set; }

        // For Paid lecture --> can't Preview
        public bool IsPreview { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }
    }
}

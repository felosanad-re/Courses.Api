using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class CourseRatingRequest
    {
        [Required]
        [Range(1, 5)]
        public int RatingValue { get; set; }
        public string? Comment { get; set; }
    }
}

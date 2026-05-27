using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class CreatedCourseRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Image { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int CourseTypeId { get; set; }

        public bool IsPaid { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}

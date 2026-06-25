using Courses.Core.Models.Courses;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class UpdatedCourseRequest
    {
        [Required]
        [MinLength(3)]
        [MaxLength(300)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public CourseStatus Status { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CourseCategoryId { get; set; }

        public bool IsPaid { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
    }
}

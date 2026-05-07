using System.ComponentModel.DataAnnotations;

namespace Courses.Core.ModelsDTO.RequestDTO.Courses
{
    public class UpdatedCourseRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }

        public bool IsPaid { get; set; }

        public decimal Price { get; set; }
        [Required]
        public int InstructorId { get; set; }
    }
}

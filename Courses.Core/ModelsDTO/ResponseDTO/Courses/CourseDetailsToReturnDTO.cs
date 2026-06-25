using Courses.Core.ModelsDTO.ResponseDTO.Sections;

namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CourseDetailsToReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }

        public string CourseCategory { get; set; } // NFP
        public int CourseCategoryId { get; set; }

        public string InstructorName { get; set; } // NFP
        public int InstructorId { get; set; }

        public IReadOnlyList<SectionToReturnDTO> Sections { get; set; }
    }
}

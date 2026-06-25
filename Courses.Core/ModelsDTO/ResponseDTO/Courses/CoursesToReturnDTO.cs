namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CoursesToReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }

        public string Status { get; set; }

        public string CourseCategory { get; set; } // NFP
        public int CourseCategoryId { get; set; }
    }
}

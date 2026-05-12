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

        public string CourseType { get; set; } // NFP
        public int CourseTypeId { get; set; }
    }
}

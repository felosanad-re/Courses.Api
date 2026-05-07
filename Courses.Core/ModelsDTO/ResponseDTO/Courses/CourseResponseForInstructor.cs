namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CourseResponseForInstructor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }

        public int InstructorId { get; set; }
    }
}

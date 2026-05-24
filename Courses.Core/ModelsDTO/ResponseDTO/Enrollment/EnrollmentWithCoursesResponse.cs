namespace Courses.Core.ModelsDTO.ResponseDTO.Enrollment
{
    public class EnrollmentWithCoursesResponse
    {
        public int Id { get; set; } // EnrollmentId
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }

        public int InstructorId { get; set; }

        public string CourseType { get; set; }
    }
}

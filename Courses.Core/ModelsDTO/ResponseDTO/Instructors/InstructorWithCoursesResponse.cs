namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class InstructorWithCoursesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public int CourseTypeId { get; set; }
        public int TotalEnrollment { get; set; } // Num Of Students
        public decimal TotalRevenues { get; set; }
        public DateTime? FirstEnrollment { get; set; }
        public DateTime? LastEnrollment { get; set; }
    }
}

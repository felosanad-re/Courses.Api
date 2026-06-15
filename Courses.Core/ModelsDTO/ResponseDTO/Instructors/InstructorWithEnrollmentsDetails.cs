namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class InstructorWithEnrollmentsDetails
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}

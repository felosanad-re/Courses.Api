namespace Courses.Core.ModelsDTO.ResponseDTO.DashboardInstructor
{
    public class DashboardInstructorDTO
    {
        public int TotalCourses { get; set; }
        public int TotalNewCoursesInMonth { get; set; }
        public int TotalStudents { get; set; }
        public int NewTotalStudentsInMonth { get; set; }
        public decimal TotalRevenues { get; set; }
        public decimal NewTotalRevenuesInMonth { get; set; }
    }
}

namespace Courses.Core.ModelsDTO.ResponseDTO.AdminDashboard
{
    public class AdminDashboardStatsResponse
    {
        public int Users { get; set; }
        public int Students { get; set; }
        public int Instructors { get; set; }
        public int Courses { get; set; }
        public int PublishedCourses { get; set; }
        public decimal Revenue { get; set; }
    }
}

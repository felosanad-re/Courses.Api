using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.ModelsDTO.ResponseDTO.Analyses
{
    public class InstructorAnalyticsDto
    {
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public int TotalStudents { get; set; }
        public decimal TotalRevenue { get; set; }
        public double? AverageCourseRating { get; set; } // Coming After Add Rating in course

        // After Add Admin Services For Accepted and Reject Courses
        public int? PublishedCourses { get; set; }
        public int? DraftCourses { get; set; }

        public CourseAnalyticDTO? TopCourseSelling { get; set; }
        public CourseAnalyticDTO? TopCourseRating { get; set; } // Coming After Add Rating in course
    }
}

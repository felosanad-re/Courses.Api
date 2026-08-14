namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class InstructorActivitiesResponse
    {
        public InstructorActivityType Type { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? StudentName { get; set; }

        public string? CourseTitle { get; set; }

        public decimal? Amount { get; set; }

        public int? Rating { get; set; }
    }
}

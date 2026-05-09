namespace Courses.Core.ModelsDTO.RequestDTO.Instructors
{
    public class ApplyInstructorRequest
    {
        public string Bio { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }
    }
}
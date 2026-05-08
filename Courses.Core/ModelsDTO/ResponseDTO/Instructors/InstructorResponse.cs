using Courses.Core.ModelsDTO.ResponseDTO.Courses;

namespace Courses.Core.ModelsDTO.ResponseDTO.Instructors
{
    public class InstructorResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Date of birth used to calculate Age dynamically
        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        // The instructor's area of expertise (e.g., "Web Development", "Data Science")
        public string Specialization { get; set; }

        public string UserId { get; set; }

        // All courses created by this instructor (one-to-many)
        public List<CourseResponse> Courses { get; set; }
    }
}

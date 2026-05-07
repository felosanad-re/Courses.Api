using Courses.Core.Models.Courses;
using Courses.Core.Models.Enrollments;
using Courses.Core.Models.Instructors;

namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CourseResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }


        public string InstructorName { get; set; }
        public int InstructorId { get; set; }


        // The category/type of this course (many-to-one)
        public string CourseType { get; set; }
        public int CourseTypeId { get; set; }


        public List<Enrollment> Enrollments { get; set; }

        public List<Section> Sections { get; set; }
    }
}

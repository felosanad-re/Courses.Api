using Courses.Core.Models.Enrollments;
using Courses.Core.ModelsDTO.ResponseDTO.Enrollment;
using Courses.Core.ModelsDTO.ResponseDTO.Sections;

namespace Courses.Core.ModelsDTO.ResponseDTO.Courses
{
    public class CourseResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }


        public string InstructorName { get; set; } // NFP
        public int InstructorId { get; set; }


        public string CourseType { get; set; } // NFP
        public int CourseTypeId { get; set; }


        public List<EnrollmentResponse> Enrollments { get; set; }

        public List<SectionResponse> Sections { get; set; }
    }
}

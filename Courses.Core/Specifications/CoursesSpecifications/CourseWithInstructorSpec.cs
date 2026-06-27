using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CourseWithInstructorSpec : BaseSpecifications<Course>
    {
        public CourseWithInstructorSpec(IEnumerable<int> coursesIds)
            : base(c => coursesIds.Contains(c.Id))
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseCategory);
            IncludesString.Add("Sections.Lectures");
        }

        public CourseWithInstructorSpec(string userId, CoursesParams @params)
            :base(c => 
                (c.Instructor.UserId == userId) && 
                (string.IsNullOrEmpty(@params.Search) || c.Name.ToLower().Contains(@params.Search.Trim().ToLower()))
            )
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseCategory);

            AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);
        }

        public CourseWithInstructorSpec(int id, int instructorId)
            : base(c =>
                    (c.Id == id) && (c.Instructor.Id == instructorId)
                  )
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseCategory);
            Includes.Add(c => c.Sections);
            IncludesString.Add("Sections.Lectures");
        }

        public CourseWithInstructorSpec(IEnumerable<int> coursesIds, int instructorId)
            :base(c => 
                (coursesIds.Contains(c.Id))
                &&
                (c.Instructor.Id == instructorId)
            )
        {
            Includes.Add(c => c.Instructor);
            Includes.Add(c => c.CourseCategory);
            IncludesString.Add("Sections.Lectures");
        }

        public CourseWithInstructorSpec(int instructorId, CourseType type, string? search)
            :base(x =>
                (x.InstructorId == instructorId)&&
                (x.Type == type)&&
                (string.IsNullOrEmpty(search) || x.Name.Contains(search))
            )
        {

        }
    }
}

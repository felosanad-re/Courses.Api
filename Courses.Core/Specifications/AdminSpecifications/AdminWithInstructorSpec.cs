using Courses.Core.Models.Instructors;
using Courses.Core.ModelsDTO.RequestDTO.Instructors;

namespace Courses.Core.Specifications.AdminSpecifications
{
    public class AdminWithInstructorSpec : BaseSpecifications<Instructor>
    {
        public AdminWithInstructorSpec(InstructorParams param)
            : base()
        {
            Includes.Add(i => i.Courses);
            Includes.Add(i => i.ApplicationUser);
            AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);
        }

        public AdminWithInstructorSpec(int instructorId)
            : base(x => x.Id == instructorId)
        {
            Includes.Add(x => x.Courses);
            Includes.Add(x => x.ApplicationUser);
            IncludesString.Add("Courses.CourseCategory");
            IncludesString.Add("Courses.Sections");
        }
    }
}

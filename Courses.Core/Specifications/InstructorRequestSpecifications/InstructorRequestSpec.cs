using Courses.Core.Models.Instructors;

namespace Courses.Core.Specifications.InstructorRequestSpecifications
{
    public class InstructorRequestSpec : BaseSpecifications<InstructorRequest>
    {

        public InstructorRequestSpec(InstructorRequestParams param)
            :base(x => string.IsNullOrEmpty(param.Search) ||
                    x.Instructor.ApplicationUser.FullName.Contains(param.Search)
            )
        {
            Includes.Add(x => x.User);

            AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);
            AddOrderByDesc(x => x.CreatedAt);
        }

        public InstructorRequestSpec(string? userId)
            :base(x => 
            (x.UserId == userId) &&
            (x.Status == InstructorRequestStatus.Pending)
            )
        {
            Includes.Add(x => x.User);
        }
    }
}

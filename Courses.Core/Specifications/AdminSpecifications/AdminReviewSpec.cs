using Courses.Core.Models.Courses;

namespace Courses.Core.Specifications.AdminSpecifications
{
    public class AdminReviewSpec : BaseSpecifications<CourseRating>
    {
        public AdminReviewSpec(ReviewsParams param)
            :base(x =>
                (string.IsNullOrEmpty(param.Search) || x.Course.Name.Contains(param.Search))
            )
        {
            Includes.Add(x => x.Course);
            Includes.Add(x => x.Student);
            IncludesString.Add("Student.ApplicationUser");

            AddPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);

            AddOrderByDesc(x => x.CreatedAt);
            AddOrderBy(x => x.Rating);
        }

        public AdminReviewSpec(int reviewId)
            :base(x => x.Id == reviewId)
        {
            Includes.Add(x => x.Course);
            Includes.Add(x => x.Student);
            IncludesString.Add("Student.ApplicationUser");

            IsTracking = true;
        }
    }
}

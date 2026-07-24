using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.DashboardInstructor;

namespace Courses.Core.Specifications.RatingSpecifications
{
    public class RatingWithSpec : BaseSpecifications<CourseRating>
    {
        public RatingWithSpec(RatingParams ratingParams, int? instructorId, bool isCount = false)
            :base(x =>
                (x.Course.InstructorId == instructorId)&&
                (string.IsNullOrEmpty(ratingParams.Search) || x.Student.Name.Contains(ratingParams.Search))
            )
        {
            if (isCount)
                return;

            Includes.Add(x => x.Course);
            Includes.Add(x => x.Student);
            AddOrderByDesc(x => x.CreatedAt);
            AddPagination((ratingParams.PageIndex - 1) * ratingParams.PageSize, ratingParams.PageSize);
        }

        public RatingWithSpec()
            :base()
        {
            Includes.Add(x => x.Course);
            Includes.Add(x => x.Student);
            AddOrderByDesc(x => x.CreatedAt);
            Take = 5;
            Skip = 0;
            IsTracking = false;
        }
    }
}

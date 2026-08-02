using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.Specifications.Options;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesWithSpec : BaseSpecifications<Course>
    {
        public CoursesWithSpec(CoursesParams @params)
            :base(x =>
                (string.IsNullOrEmpty(@params.Search) || x.Name.ToLower().Contains(@params.Search.Trim().ToLower())) &&
                (!@params.Type.HasValue || x.CourseCategoryId == @params.Type)&&
                (x.Status == CourseStatus.Published)
            )
        {
            Includes.Add(c => c.CourseCategory);

            AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);

            AddSorting(@params);
        }

        public CoursesWithSpec(CoursesParams @params, CourseType courseType)
            :base(x =>
                (string.IsNullOrEmpty(@params.Search) || x.Name.ToLower().Contains(@params.Search.Trim().ToLower())) &&
                (!@params.Type.HasValue || x.CourseCategoryId == @params.Type)&&
                (x.Type == courseType)&&
                (x.Status == CourseStatus.Published)
            )
        {
            Includes.Add(c => c.CourseCategory);

            AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);

            AddSorting(@params);
        }

        public CoursesWithSpec(int courseId)
            : base(x => x.Id == courseId)
        {
            Includes.Add(c => c.CourseCategory);
            Includes.Add(c => c.Sections);
            IncludesString.Add("Sections.Lectures");
            Includes.Add(c => c.Instructor);
        }

        public CoursesWithSpec(int courseId, CourseType type)
            : base(x => x.Id == courseId && x.Type == type)
        {

        }
        #region Helper method
        private void AddSorting(CoursesParams @params)
        {
            if (Enum.TryParse<CourseSortingOptions>(@params.Sort, true, out var sortingOptions))
            {
                switch (sortingOptions)
                {
                    case CourseSortingOptions.PriceAsc:
                        AddOrderBy(x => x.Price);
                        break;

                    case CourseSortingOptions.PriceDesc:
                        AddOrderByDesc(x => x.Price);
                        break;

                    case CourseSortingOptions.Rating:
                        AddOrderByDesc(x => x.AverageRating);
                        break;

                    case CourseSortingOptions.Newest:
                        AddOrderByDesc(x => x.CreatedAt);
                        break;

                    case CourseSortingOptions.Popular:
                        AddOrderByDesc(x => x.Enrollments.Count);
                        break;

                    default:
                        AddOrderBy(x => x.Name);
                        break;
                }
            }
        }
        #endregion
    }
}

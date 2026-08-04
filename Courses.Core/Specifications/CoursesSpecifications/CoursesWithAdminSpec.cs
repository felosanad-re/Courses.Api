using Courses.Core.Models.Courses;
using Courses.Core.ModelsDTO.RequestDTO.Courses;
using Courses.Core.Specifications.Options;

namespace Courses.Core.Specifications.CoursesSpecifications
{
    public class CoursesWithAdminSpec : BaseSpecifications<Course>
    {
        public CoursesWithAdminSpec(CoursesParams @params, CourseType? courseType, CourseStatus? status)
            : base(x =>
                (string.IsNullOrEmpty(@params.Search) || x.Name.ToLower().Contains(@params.Search.Trim().ToLower())) &&
                (!@params.Type.HasValue || x.CourseCategoryId == @params.Type) &&
                (!courseType.HasValue || x.Type == courseType) &&
                (!status.HasValue || x.Status == status)
            )
        {
            Includes.Add(c => c.CourseCategory);

            AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);

            AddSorting(@params);
        }

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
    }
}

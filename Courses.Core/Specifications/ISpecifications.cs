using Courses.Core.Models;
using System.Linq.Expressions;

namespace Courses.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseModel
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; }
        public List<string> IncludesString { get; set; }
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }

        // Tracking
        public bool IsTracking { get; set; }

        // pagination
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagination { get; set; }
    }
}

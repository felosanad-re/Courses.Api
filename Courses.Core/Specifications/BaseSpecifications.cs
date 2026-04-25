using Courses.Core.Models;
using System.Linq.Expressions;

namespace Courses.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseModel
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludesString { get; set; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public bool IsTracking { get; set; } = true;
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPagination { get; set; }

        // Set Criteria null
        public BaseSpecifications()
        {
        }

        public BaseSpecifications(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        // Order By
        public void AddOrderBy(Expression<Func<T, object>> orderBy)
        {
            OrderBy = orderBy;
        }

        public void AddOrderByDesc(Expression<Func<T, object>> orderByDesc)
        {
            OrderByDesc = orderByDesc;
        }

        public void AddPagination(int skip, int take)
        {
            IsPagination = true;
            Take = take;
            Skip = skip;
        }
    }
}

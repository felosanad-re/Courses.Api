using Courses.Core.Models;
using Courses.Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Repo.Specifications
{
    public static class EvaluateSpec<T> where T : BaseModel
    {
        public static IQueryable<T> GetQuery(IQueryable<T> initialQuery, ISpecifications<T> spec)
        {
            var query = initialQuery;
            // Add Criteria 
            if (spec.Criteria != null) query = query.Where(spec.Criteria);
            // Set Order By
            if (spec.OrderBy != null) query = query.OrderBy(spec.OrderBy);
            // set order by desc
            if (spec.OrderByDesc != null) query = query.OrderByDescending(spec.OrderByDesc);
            // Is Tracking
            if (!spec.IsTracking) query = query.AsNoTracking();
            // Pagination
            if (spec.IsPagination) query = query.Skip(spec.Skip).Take(spec.Take);
            // Includes
            query = spec.Includes.Aggregate(query, (baseQuery, nextQuery) => baseQuery.Include(nextQuery));
            // ThenIncludes
            if (spec.IncludesString.Any())
                query = spec.IncludesString.Aggregate(query, (baseQuery, includes) => baseQuery.Include(includes));

            return query;
        }
    }
}

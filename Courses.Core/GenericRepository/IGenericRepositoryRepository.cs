using Courses.Core.Models;
using Courses.Core.Specifications;
using System.Linq.Expressions;

namespace Courses.Core.GenericRepository
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecifications<T> spec);
        Task<T?> GetAsyncSpec(ISpecifications<T> spec);
        Task<int> GetCountAsyncSpec(ISpecifications<T> spec);

        Task<decimal> GetSumAsyncSpec(ISpecifications<T> spec, Expression<Func<T, decimal>> selector);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
    }
}

using Courses.Core.GenericRepository;
using Courses.Core.Models;
using Courses.Core.Specifications;
using Courses.Repo.Data;
using Courses.Repo.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Courses.Repo.GenericRepositories
{
    public class GenericRepo<T> : IGenericRepository<T> where T : BaseModel
    {
        protected readonly CoursesDbContext _dbContext;

        public GenericRepo(CoursesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _dbContext.Set<T>().AsNoTracking().ToListAsync();

        public async Task<T?> GetAsync(int id)
            => await _dbContext.Set<T>().FindAsync(id);


        public async Task<IReadOnlyList<T>> GetAllAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).ToListAsync();

        public async Task<T?> GetAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).FirstOrDefaultAsync();

        public async Task<int> GetCountAsyncSpec(ISpecifications<T> spec)
            => await AddSpecifications(spec).CountAsync();

        public async Task AddAsync(T entity)
            => await _dbContext.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(IEnumerable<T> entities)
            => await _dbContext.AddRangeAsync(entities);

        public void Update(T entity)
            => _dbContext.Set<T>().Update(entity);

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbContext.Set<T>().Update(entity);
        }

        private IQueryable<T> AddSpecifications(ISpecifications<T> spec)
        {
            return EvaluateSpec<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}

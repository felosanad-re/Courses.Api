using Courses.Core.GenericRepository;
using Courses.Core.Models;
using Courses.Core.UnitOfWork;
using Courses.Repo.Data;
using Courses.Repo.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courses.Repo.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly CoursesDbContext _dbContext;
        protected readonly Dictionary<Type, object> _repository = new();

        public UnitOfWork(CoursesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<T> CreateRepository<T>() where T : BaseModel
        {
            var type = typeof(T);
            if (!_repository.ContainsKey(type))
            {
                _repository[type] = new GenericRepo<T>(_dbContext);
            }
            return (IGenericRepository<T>)_repository[type];
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();
    }
}

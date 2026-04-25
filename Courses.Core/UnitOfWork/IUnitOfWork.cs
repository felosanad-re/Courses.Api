using Courses.Core.GenericRepository;
using Courses.Core.Models;

namespace Courses.Core.UnitOfWork
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepository<T> CreateRepository<T>() where T : BaseModel;
        Task<int> CompleteAsync();
    }
}

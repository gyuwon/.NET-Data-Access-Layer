using System;
using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task SaveChangesAsync();
    }
}

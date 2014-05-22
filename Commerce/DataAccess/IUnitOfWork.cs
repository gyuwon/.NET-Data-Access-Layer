using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task SaveChangesAsync();
    }
}

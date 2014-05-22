using System.Linq;
using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        IQueryable<TEntity> Query { get; set; }
        TEntity Create(TEntity entity);
        TEntity Update(TEntity entity);
        TEntity Remove(TEntity entity);
    }
}

using System.Linq;
using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        IQueryable<TEntity> Query { get; }
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }
}

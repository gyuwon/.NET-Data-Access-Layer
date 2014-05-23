using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public class DbContextRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private DbContext _dbContext;
        private DbSet<TEntity> _dbSet;

        public DbContextRepository(DbContext dbContext)
        {
            this._dbContext = dbContext;
            this._dbSet = dbContext.Set<TEntity>();
        }

        public Task<TEntity> FindAsync(params object[] keyValues)
        {
            return this._dbSet.FindAsync(keyValues);
        }

        public IQueryable<TEntity> Query
        {
            get
            {
                return this._dbSet;
            }
        }

        public void Create(TEntity entity)
        {
            this._dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            this._dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(TEntity entity)
        {
            this._dbSet.Remove(entity);
        }
    }
}
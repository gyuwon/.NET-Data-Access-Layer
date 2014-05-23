using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Commerce.DataAccess
{
    public class DbContextUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext, new()
    {
        private TDbContext _dbContext;
        private Dictionary<Type, object> _repositories;

        public DbContextUnitOfWork()
        {
            this._dbContext = new TDbContext();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (this._repositories == null)
            {
                this._repositories = new Dictionary<Type, object>();
            }
            object repository;
            if (this._repositories.TryGetValue(typeof(TEntity), out repository) == false)
            {
                repository = new DbContextRepository<TEntity>(this._dbContext);
                this._repositories[typeof(TEntity)] = repository;
            }
            return (IRepository<TEntity>)repository;
        }

        public Task SaveChangesAsync()
        {
            return this._dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
}
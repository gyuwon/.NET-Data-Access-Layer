using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Commerce.Models;

namespace Commerce.DataAccess
{
    public class ItemDataAccess : IDisposable
    {
        private ApplicationDbContext _dbContext = new ApplicationDbContext();

        public IQueryable<Item> Query
        {
            get
            {
                return this._dbContext.Items;
            }
        }

        public Task<Item> FindAsync(long id)
        {
            return this._dbContext.Items.FindAsync(id);
        }

        public void Create(Item entity)
        {
            this._dbContext.Items.Add(entity);
        }

        public void Update(Item entity)
        {
            this._dbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Item entity)
        {
            this._dbContext.Items.Remove(entity);
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
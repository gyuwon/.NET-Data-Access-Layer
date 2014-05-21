using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Commerce.Models
{
    public class ItemDataAccess
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public ItemDataAccess(ApplicationDbContext db)
        {
            this._db = db;
        }

        public IQueryable<Item> Query
        {
            get
            {
                return this._db.Items;
            }
        }

        public Task<Item> FindAsync(long id)
        {
            return this._db.Items.FindAsync(id);
        }

        public async Task<Item> CreateAsync(Item item)
        {
            this._db.Items.Add(item);
            await this._db.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateAsync(Item item)
        {
            this._db.Entry(item).State = EntityState.Modified;
            try
            {
                await this._db.SaveChangesAsync();
                return item;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (null == this._db.Items.Find(item.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Item> DeleteAsync(long id)
        {
            Item item = await this._db.Items.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            this._db.Items.Remove(item);
            await this._db.SaveChangesAsync();
            return item;
        }
    }
}
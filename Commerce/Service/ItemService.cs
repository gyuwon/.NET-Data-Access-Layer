using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Commerce.Models;

namespace Commerce.Service
{
    public class ItemService : IDisposable
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await this._db.Items.ToListAsync();
        }

        public async Task<Item> GetItemAsync(long id)
        {
            Item item = await this._db.Items.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            var query = from c in this._db.Comments
                        where c.ItemId == item.Id
                        orderby c.CreatedAt
                        select new { Comment = c, AuthorName = c.Author.UserName };
            await query.ForEachAsync(e => e.Comment.AuthorName = e.AuthorName);
            return item;
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            this._db.Items.Add(item);
            await this._db.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
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

        public async Task<Item> DeleteItemAsync(long id)
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

        public async Task<Comment> CreateCommentAsync(string authorId, long itemId, string content)
        {
            Item item = await this._db.Items.FindAsync(itemId);
            if (item == null)
            {
                return null;
            }
            Comment comment = new Comment
            {
                ItemId = itemId,
                AuthorId = authorId,
                Content = content,
                CreatedAt = DateTime.Now
            };
            this._db.Comments.Add(comment);
            await this._db.SaveChangesAsync();
            return comment;
        }

        public void Dispose()
        {
            this._db.Dispose();
        }
    }
}
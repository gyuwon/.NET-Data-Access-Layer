using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Commerce.Models;

namespace Commerce.Services
{
    public class ItemService : IDisposable
    {
        private ApplicationDbContext _db;
        private ItemDataAccess _items;
        private CommentDataAccess _comments;

        public ItemService()
        {
            this._db = new ApplicationDbContext();
            this._items = new ItemDataAccess(this._db);
            this._comments = new CommentDataAccess(this._db);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await this._items.Query.ToListAsync();
        }

        public async Task<Item> GetItemAsync(long id)
        {
            Item item = await this._items.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            var query = from c in this._comments.Query
                        where c.ItemId == item.Id
                        orderby c.CreatedAt
                        select new { Comment = c, AuthorName = c.Author.UserName };
            await query.ForEachAsync(e => e.Comment.AuthorName = e.AuthorName);
            return item;
        }

        public Task<Item> CreateItemAsync(Item item)
        {
            return this._items.CreateAsync(item);
        }

        public Task<Item> UpdateItemAsync(Item item)
        {
            return this._items.UpdateAsync(item);
        }

        public Task<Item> DeleteItemAsync(long id)
        {
            return this._items.DeleteAsync(id);
        }

        public async Task<Comment> CreateCommentAsync(string authorId, long itemId, string content)
        {
            Item item = await this._items.FindAsync(itemId);
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
            return await this._comments.CreateAsync(comment);
        }

        public void Dispose()
        {
            this._db.Dispose();
        }
    }
}
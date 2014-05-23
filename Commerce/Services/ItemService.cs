using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Commerce.DataAccess;
using Commerce.Models;

namespace Commerce.Services
{
    public class ItemService : IDisposable
    {
        private IUnitOfWork _unitOfWork;
        private IRepository<Item> _items;
        private IRepository<Comment> _comments;

        public ItemService()
            : this(new DbContextUnitOfWork<ApplicationDbContext>())
        {
        }

        public ItemService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
            this._items = unitOfWork.GetRepository<Item>();
            this._comments = unitOfWork.GetRepository<Comment>();
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            return await this._items.Query.ToListAsync();
        }

        public async Task<Item> GetItemAsync(long id)
        {
            Item item = await this._items.FindAsync(id);
            var query = from c in this._comments.Query
                        where c.ItemId == id
                        orderby c.CreatedAt
                        select new
                        {
                            Comment = c,
                            AuthorName = c.Author.UserName
                        };
            List<Comment> comments = new List<Comment>();
            await query.ForEachAsync(e =>
            {
                Comment comment = e.Comment;
                comment.AuthorName = e.AuthorName;
                comments.Add(comment);
            });
            item.Comments = comments;
            return item;
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            this._items.Create(item);
            await this._unitOfWork.SaveChangesAsync();
            return item;
        }

        public async Task<Item> UpdateItemAsync(Item item)
        {
            if (false == await this._items.Query.AnyAsync(e => e.Id == item.Id))
            {
                return null;
            }
            this._items.Update(item);
            await this._unitOfWork.SaveChangesAsync();
            return item;
        }

        public async Task<Item> DeleteItemAsync(long id)
        {
            Item item = await this._items.FindAsync(id);
            if (item == null)
            {
                return null;
            }
            this._items.Remove(item);
            await this._unitOfWork.SaveChangesAsync();
            return item;
        }

        public async Task<Comment> CreateCommentAsync(long itemId, string authorId, string content)
        {
            if (false == await this._items.Query.AnyAsync(e => e.Id == itemId))
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
            this._comments.Create(comment);
            await this._unitOfWork.SaveChangesAsync();
            return comment;
        }

        public void Dispose()
        {
            this._unitOfWork.Dispose();
        }
    }
}

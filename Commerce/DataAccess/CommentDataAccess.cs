using System;
using System.Linq;
using System.Threading.Tasks;
using Commerce.Models;

namespace Commerce.DataAccess
{
    public class CommentDataAccess : IDisposable
    {
        private ApplicationDbContext _dbContext = new ApplicationDbContext();

        public IQueryable<Comment> Query
        {
            get
            {
                return this._dbContext.Comments;
            }
        }

        public void Create(Comment entity)
        {
            this._dbContext.Comments.Add(entity);
        }

        public Task SaveChagesAsync()
        {
            return this._dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            this._dbContext.Dispose();
        }
    }
}
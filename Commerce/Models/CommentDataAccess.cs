using System.Linq;
using System.Threading.Tasks;

namespace Commerce.Models
{
    public class CommentDataAccess
    {
        private ApplicationDbContext _db = new ApplicationDbContext();

        public CommentDataAccess(ApplicationDbContext db)
        {
            this._db = db;
        }

        public IQueryable<Comment> Query
        {
            get
            {
                return this._db.Comments;
            }
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            this._db.Comments.Add(comment);
            await this._db.SaveChangesAsync();
            return comment;
        }
    }
}
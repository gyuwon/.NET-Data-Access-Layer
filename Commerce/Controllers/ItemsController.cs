using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Commerce.Models;
using Commerce.Services;
using Microsoft.AspNet.Identity;

namespace Commerce.Controllers
{
    public class ItemsController : ApiController
    {
        private ItemService _service = new ItemService();

        // GET: api/Items
        public Task<IEnumerable<Item>> GetItems()
        {
            return this._service.GetItemsAsync();
        }

        // GET: api/Items/5
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> GetItem(long id)
        {
            Item item = await this._service.GetItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        // PUT: api/Items/5
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutItem(long id, Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest();
            }

            if (null == await this._service.UpdateItemAsync(item))
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Items
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> PostItem(Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await this._service.CreateItemAsync(item);

            return CreatedAtRoute("DefaultApi", new { id = item.Id }, item);
        }

        // DELETE: api/Items/5
        [Authorize(Roles = "Administrator")]
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> DeleteItem(long id)
        {
            Item item = await this._service.DeleteItemAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [Authorize]
        [Route("api/Items/{id}/Comments")]
        [ResponseType(typeof(Comment))]
        public async Task<IHttpActionResult> PostComment(long id, CommentBindingModel comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment entity = await this._service.CreateCommentAsync(this.User.Identity.GetUserId(), id, comment.Content);

            return Ok(entity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
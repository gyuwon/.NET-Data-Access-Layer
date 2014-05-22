using System;
using System.Linq;
using System.Threading.Tasks;
using Commerce.DataAccess;
using Commerce.Models;
using Commerce.Services;
using EntityFramework.Testing.Moq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Commerce.Tests.Services
{
    [TestClass]
    public class ItemServiceTest
    {
        private Random _random;
        private long _itemId;
        private string _userId;
        private Mock<IRepository<Item>> _items;
        private Mock<IRepository<Comment>> _comments;
        private Mock<IUnitOfWork> _unitOfWork;
        private ItemService _service;

        [TestInitialize]
        public void Initialize()
        {
            this._random = new Random();
            this._itemId = this._random.Next();
            this._userId = Guid.NewGuid().ToString();
            this._items = new Mock<IRepository<Item>>();
            this._comments = new Mock<IRepository<Comment>>();
            this._unitOfWork = new Mock<IUnitOfWork>();
            this._unitOfWork.Setup(u => u.GetRepository<Item>()).Returns(this._items.Object);
            this._unitOfWork.Setup(u => u.GetRepository<Comment>()).Returns(this._comments.Object);
            this._service = new ItemService(this._unitOfWork.Object);
        }

        [TestMethod]
        public async Task GetItemAsync_Should_ReturnFindAsync()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);
            var dbSet = new MockDbSet<Comment>().SetupSeedData(Enumerable.Empty<Comment>()).SetupLinq();
            this._comments.SetupGet(r => r.Query).Returns(dbSet.Object);

            // Act
            Item result = await this._service.GetItemAsync(this._itemId);

            // Assert
            this._items.Verify(r => r.FindAsync(this._itemId), Times.Once());
            result.Should().Be(item);
        }

        [TestMethod]
        public async Task GetItemAsync_Should_ReturnItemWithSortedRelatedComments()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);
            var comments = Enumerable.Range(0, 100).Select(_ =>
                new Comment
                {
                    ItemId = this._random.Next() % 2 == 0 ? this._itemId : this._itemId + 1,
                    Author = new ApplicationUser { UserName = "Homer" },
                    CreatedAt = new DateTime(2014, 05, this._random.Next(31) + 1)
                }
            ).ToList();
            var dbSet = new MockDbSet<Comment>().SetupSeedData(comments).SetupLinq();
            this._comments.SetupGet(r => r.Query).Returns(dbSet.Object);

            // Act
            Item result = await this._service.GetItemAsync(this._itemId);

            // Assert
            this._comments.VerifyGet(r => r.Query, Times.Once());
            result.Should().NotBeNull();
            result.Comments.Should().NotBeNull();
            result.Comments.ToList().ForEach(c => c.ItemId.Should().Be(this._itemId));
            result.Comments.Should().BeInAscendingOrder(c => c.CreatedAt);
        }

        [TestMethod]
        public async Task GetItemAsync_Should_SetCommentAuthorNameFromAuthor()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);
            var authorName = "Homer";
            var comment = new Comment
            {
                ItemId = this._itemId,
                Author = new ApplicationUser { UserName = authorName }
            };
            var dbSet = new MockDbSet<Comment>().SetupSeedData(new[] { comment }).SetupLinq();
            this._comments.SetupGet(r => r.Query).Returns(dbSet.Object);

            // Act
            Item result = await this._service.GetItemAsync(this._itemId);

            // Assert
            result.Comments.Should().HaveCount(1);
            result.Comments.Single().AuthorName.Should().Be(authorName);
        }

        [TestMethod]
        public async Task CreateItemAsync_Should_CallCreateSaveChangesAsync()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.Create(item)).Returns(item);

            // Act
            Item result = await this._service.CreateItemAsync(item);

            // Assert
            result.Should().Be(item);
            this._items.Verify(r => r.Create(item), Times.Once());
            this._unitOfWork.Verify(r => r.SaveChangesAsync(), Times.Once());
        }

        [TestMethod]
        public async Task UpdateItemAsync_Should_CallUpdateSaveChangesAsyncIfExists()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.Update(item)).Returns(item);
            this._unitOfWork.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            Item result = await this._service.UpdateItemAsync(item);

            // Assert
            result.Should().Be(item);
            this._items.Verify(r => r.Update(item), Times.Once());
            this._unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [TestMethod]
        public async Task UpdateItemAsync_Should_CallUpdateIfNotExists()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.Update(item)).Returns(() => null);
            this._unitOfWork.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            Item result = await this._service.UpdateItemAsync(item);

            // Assert
            result.Should().BeNull();
            this._items.Verify(r => r.Update(item), Times.Once());
            this._unitOfWork.Verify(r => r.SaveChangesAsync(), Times.Never());
        }

        [TestMethod]
        public async Task RemoveItemAsync_Should_CallFindAsync()
        {
            // Arrange

            // Act
            Item result = await this._service.RemoveItemAsync(this._itemId);

            // Assert
            this._items.Verify(r => r.FindAsync(this._itemId), Times.Once());
        }

        [TestMethod]
        public async Task RemoveItemAsync_Should_CallRemoveSaveChangesAsyncIfExists()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);
            this._items.Setup(r => r.Update(item)).Returns(item);
            this._unitOfWork.Setup(r => r.SaveChangesAsync()).Returns(Task.FromResult(1));

            // Act
            Item result = await this._service.RemoveItemAsync(this._itemId);

            // Assert
            this._items.Verify(r => r.Remove(item), Times.Once());
            this._unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [TestMethod]
        public async Task RemoveItemAsync_Should_ReturnNullIfNotExists()
        {
            // Arrange
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(null);

            // Act
            Item result = await this._service.RemoveItemAsync(this._itemId);

            // Assert
            result.Should().BeNull();
            this._unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never());
        }

        [TestMethod]
        public async Task CreateCommentAsync_Should_CallFindAsync()
        {
            // Arrange

            // Act
            Comment result = await this._service.CreateCommentAsync(this._itemId, this._userId, "D'oh!");

            // Assert
            this._items.Verify(r => r.FindAsync(this._itemId), Times.Once());
        }

        [TestMethod]
        public async Task CreateCommentAsync_Should_ReturnNullIfNotExists()
        {
            // Arrange
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(null);

            // Act
            Comment result = await this._service.CreateCommentAsync(this._itemId, this._userId, "D'oh!");

            // Assert
            result.Should().BeNull();
            this._unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never());
        }

        [TestMethod]
        public async Task CreateCommentAsync_Should_CallCreateSaveChangesAsyncIfExists()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);

            // Act
            Comment result = await this._service.CreateCommentAsync(this._itemId, this._userId, "D'oh!");

            // Assert
            this._comments.Verify(r => r.Create(result), Times.Once());
            this._unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once());
        }

        [TestMethod]
        public async Task CreateCommentAsync_Should_ReturnCommentIfExists()
        {
            // Arrange
            var item = new Item { Id = this._itemId };
            this._items.Setup(r => r.FindAsync(this._itemId)).ReturnsAsync(item);
            var content = "D'oh!";
            var now = DateTime.Now;

            // Act
            Comment result = await this._service.CreateCommentAsync(this._itemId, this._userId, content);

            // Assert
            result.Should().NotBeNull();
            result.ItemId.Should().Be(this._itemId);
            result.AuthorId.Should().Be(this._userId);
            result.Content.Should().Be(content);
            (result.CreatedAt - now).TotalMilliseconds.Should().BeInRange(0, 10);
        }

        [TestMethod]
        public void Dispose_Should_CallUnitOfWorkDispose()
        {
            // Arrange

            // Act
            this._service.Dispose();

            // Assert
            this._unitOfWork.Verify(u => u.Dispose(), Times.Once());
        }
    }
}

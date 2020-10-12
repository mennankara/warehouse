using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Domain.Internals.Repository.Handlers;
using Warehouse.Domain.Internals.Repository.Models;
using Warehouse.Domain.Tests.RepositoryTests.DataAccessMocks;
using Xunit;

namespace Warehouse.Domain.Tests.RepositoryTests
{
    public class AddUpdateArticlesHandlerTests
    {
        private readonly AddUpdateArticlesHandler _handler;
        private readonly DbContextMock _dbContext;

        public AddUpdateArticlesHandlerTests()
        {
            _dbContext = new DbContextMock();
            _handler = new AddUpdateArticlesHandler(
                _dbContext,
                new Mock<ILogger<AddUpdateArticlesHandler>>().Object);
        }

        [Fact]
        public async Task AddUpdateArticles_ShouldSucceed_WithNewArticle()
        {
            // Arrange
            var articleId = new Random().Next();
            var articles = new List<Article> { new Article { ArticleId = articleId } };

            // Act
            await _handler.AddUpdateArticlesAsync(articles);

            // Assert
            Assert.Equal(articleId, _dbContext.Articles.First().ArticleId);
        }

        [Fact]
        public async Task AddUpdateArticles_ShouldSucceed_WithUpdatedArticle()
        {
            // Arrange
            var articleId = new Random().Next();
            const string oldName = "Old";
            var oldQuantity = new Random().Next();
            await _dbContext.Articles.AddAsync(new Article { ArticleId = articleId, Name = oldName, StockQuantity = oldQuantity });
            await _dbContext.SaveChangesAsync();
            const string newName = "New";
            var newQuantity = new Random().Next();
            var updatedArticles = new List<Article> { new Article { ArticleId = articleId, Name = newName, StockQuantity = newQuantity } };

            // Act
            await _handler.AddUpdateArticlesAsync(updatedArticles);

            // Assert
            Assert.Single(_dbContext.Articles);
            Assert.Equal(articleId, _dbContext.Articles.First().ArticleId);
            Assert.Equal(newName, _dbContext.Articles.First().Name);
            Assert.Equal(newQuantity, _dbContext.Articles.First().StockQuantity);
        }
    }
}

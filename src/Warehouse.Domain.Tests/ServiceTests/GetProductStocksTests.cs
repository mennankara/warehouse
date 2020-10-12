using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository;
using Warehouse.Domain.Internals.Repository.Models;
using Warehouse.Domain.Internals.Service.Handlers;
using Xunit;

namespace Warehouse.Domain.Tests.ServiceTests
{
    public class GetProductStocksTests
    {
        private readonly GetProductStocksHandler _handler;
        private readonly Mock<IWarehouseRepository> _repositoryMock;

        public GetProductStocksTests()
        {
            _repositoryMock = new Mock<IWarehouseRepository>();
            _handler = new GetProductStocksHandler(
                _repositoryMock.Object,
                new Mock<ILogger<GetProductStocksHandler>>().Object);
        }

        [Fact]
        public async Task GetProductStocksAsync_ShouldFail_WithGetArticlesRepositoryError()
        {
            // Arrange
            const string message = "TestException";
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Product 1",
                    ProductArticles = new List<ProductArticle>
                    {
                        new ProductArticle { ArticleId = 1, AmountOfArticles = 1 }
                    }
                }
            };

            _repositoryMock.Setup(r => r.GetProductsAsync()).ReturnsAsync(products);
            _repositoryMock.Setup(r => r.GetArticlesAsync(null)).ThrowsAsync(new WarehouseException(message));

            // Act
            var result = await _handler.GetProductStocksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Single(result.Errors);
            Assert.Equal(message, result.Errors[0].Message);
        }

        [Fact]
        public async Task GetProductStocksAsync_ShouldFail_WithGetProductsRepositoryError()
        {
            // Arrange
            const string message = "TestException";

            _repositoryMock.Setup(r => r.GetProductsAsync()).ThrowsAsync(new WarehouseException(message));

            // Act
            var result = await _handler.GetProductStocksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
            Assert.Single(result.Errors);
            Assert.Equal(message, result.Errors[0].Message);
        }

        [Theory]
        [InlineData(5, 3, 20, 20, 4)]
        [InlineData(5, 3, 4, 20, 0)]
        public async Task GetProductStocksAsync_ShouldSucceed(
            int productsAmountOfArticles1,
            int productsAmountOfArticles2,
            int article1StockQuantity,
            int article2StockQuantity,
            int expectedProductStock
            )
        {
            // Arrange
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Product 1",
                    ProductArticles = new List<ProductArticle>
                    {
                        new ProductArticle { ArticleId = 1, AmountOfArticles = productsAmountOfArticles1 },
                        new ProductArticle { ArticleId = 2, AmountOfArticles = productsAmountOfArticles2 },
                    }
                }
            };

            var articles = new List<Article>
            {
                new Article { ArticleId = 1, Name = "Article 1", StockQuantity = article1StockQuantity },
                new Article { ArticleId = 2, Name = "Article 1", StockQuantity = article2StockQuantity }
            };

            _repositoryMock.Setup(r => r.GetProductsAsync()).ReturnsAsync(products);
            _repositoryMock.Setup(r => r.GetArticlesAsync(null)).ReturnsAsync(articles);

            // Act
            var result = await _handler.GetProductStocksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
            Assert.Single(result.Response);
            Assert.Equal(expectedProductStock, result.Response[0].Quantity);
        }
    }
}

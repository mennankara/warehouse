using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository;
using Warehouse.Domain.Internals.Repository.Models;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;

namespace Warehouse.Domain.Internals.Service.Handlers
{
    internal class OrderProductHandler : IOrderProductHandler
    {
        private readonly IWarehouseRepository _repository;
        private readonly ILogger<OrderProductHandler> _logger;

        public OrderProductHandler(
            IWarehouseRepository repository,
            ILogger<OrderProductHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> OrderProductAsync(string name)
        {
            _logger.LogTrace("Starting OrderProductAsync");

            var resultBuilder = new ResultBuilder();

            try
            {
                var product = await _repository.GetProductAsync(name);
                if (product == null)
                {
                    _logger.LogError($"Product not found: {name}");
                    return resultBuilder
                        .WithError("Product not found")
                        .WithFailure(HttpStatusCode.NotFound)
                        .Build();
                }
                
                if (!await IsInStock(product))
                {
                    _logger.LogError($"Product not in stock: {product.Name}");
                    return resultBuilder
                        .WithError("Product is not in stock")
                        .WithFailure(HttpStatusCode.NotFound)
                        .Build();
                }

                await UpdateArticleStocks(product);

                // TODO: Possibly record the order (not in scope of the assignment)

            }
            catch (WarehouseException e)
            {
                return resultBuilder
                    .FromException(e)
                    .Build();
            }

            return resultBuilder
                .WithSuccess()
                .Build();
        }

        private async Task<bool> IsInStock(Product product)
        {
            var productsArticleIds = product.ProductArticles.Select(pa => pa.ArticleId).ToList();
            var articlesMap = (await _repository.GetArticlesAsync(productsArticleIds)).ToDictionary(a => a.ArticleId);

            foreach (var productArticle in product.ProductArticles)
            {
                var article = articlesMap.ContainsKey(productArticle.ArticleId)
                    ? articlesMap[productArticle.ArticleId]
                    : null;
                if (article == null || article.StockQuantity < productArticle.AmountOfArticles)
                {
                    return false;
                }
            }
            return true;
        }

        private async Task UpdateArticleStocks(Product product)
        {
            // The revert logic here can be replaced with a database transaction, which would require the repository implementation
            // supporting the transactions, which I decided not to do due to limited time I have to deliver the solution

            var updatesToRevert = new List<UpdatedArticle>();
            foreach (var productArticle in product.ProductArticles)
            {
                try
                {
                    await _repository.DecreaseArticleQuantityAsync(productArticle.ArticleId,
                        productArticle.AmountOfArticles);
                }
                catch (WarehouseException e)
                {
                    try
                    {
                        // Try to revert the successful article stock updates
                        foreach (var updatedArticle in updatesToRevert)
                        {
                            await _repository.IncreaseArticleQuantityAsync(updatedArticle.ArticleId, updatedArticle.Quantity);
                        }
                    }
                    catch (Exception exception)
                    {
                        _logger.LogCritical(exception, "Unable to revert the order");
                    }
                    _logger.LogError($"Quantity deduction failed for article {productArticle.ArticleId} with quantity {productArticle.AmountOfArticles}", e);
                    throw;
                }

                updatesToRevert.Add(new UpdatedArticle
                {
                    ArticleId = productArticle.ArticleId,
                    Quantity = productArticle.AmountOfArticles
                });
            }
        }

        private struct UpdatedArticle
        {
            internal int ArticleId { get; set; }
            internal int Quantity { get; set; }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository;
using Warehouse.Domain.Internals.Repository.Models;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;
using Warehouse.Domain.Models.Responses;

namespace Warehouse.Domain.Internals.Service.Handlers
{
    internal class GetProductStocksHandler : IGetProductStocksHandler
    {
        private readonly IWarehouseRepository _repository;
        private readonly ILogger<GetProductStocksHandler> _logger;

        public GetProductStocksHandler(
            IWarehouseRepository repository,
            ILogger<GetProductStocksHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<List<ProductStockResponse>>> GetProductStocksAsync()
        {
            _logger.LogTrace("Starting GetProductStocksAsync");

            var resultBuilder = new ResultBuilder<List<ProductStockResponse>>();

            List<Product> products;
            List<Article> articles;

            try
            {
                products = await _repository.GetProductsAsync();
                articles = await _repository.GetArticlesAsync();
            }
            catch (WarehouseException e)
            {
                return resultBuilder
                    .FromException(e)
                    .Build();
            }

            var result = GetProductStocks(products, articles);

            return resultBuilder
                .WithData(result)
                .WithSuccess()
                .Build();
        }

        private static List<ProductStockResponse> GetProductStocks(List<Product> products, List<Article> articles)
        {
            var result = new List<ProductStockResponse>();
            var articlesMap = articles.ToDictionary(a => a.ArticleId);
            foreach (var product in products.Where(p => p.ProductArticles.Any()))
            {
                var productStock = new ProductStockResponse { Name = product.Name };

                var minQuantity = int.MaxValue;
                foreach (var productArticle in product.ProductArticles)
                {
                    var articleStock = articlesMap.ContainsKey(productArticle.ArticleId) ? articlesMap[productArticle.ArticleId] : null;

                    var availableQuantity = (articleStock?.StockQuantity ?? 0) / productArticle.AmountOfArticles;

                    if (minQuantity > availableQuantity)
                    {
                        minQuantity = availableQuantity;
                    }
                }

                productStock.Quantity = minQuantity;

                result.Add(productStock);
            }

            return result;
        }
    }
}

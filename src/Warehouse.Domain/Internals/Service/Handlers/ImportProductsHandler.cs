using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository;
using Warehouse.Domain.Internals.Repository.Models;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;
using Warehouse.Domain.Internals.Service.Models;

namespace Warehouse.Domain.Internals.Service.Handlers
{
    internal class ImportProductsHandler : IImportProductsHandler
    {
        private readonly IWarehouseRepository _repository;
        private readonly ILogger<ImportProductsHandler> _logger;

        public ImportProductsHandler(
            IWarehouseRepository repository,
            ILogger<ImportProductsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> ImportAsync(Stream productsFileStream)
        {
            _logger.LogTrace("Starting to import products");

            var resultBuilder = new ResultBuilder();

            ImportedProducts importedProducts;
            try
            {
                importedProducts = await DeserializeProductsStream(productsFileStream);
            }
            catch (WarehouseException e)
            {
                return resultBuilder
                    .FromException(e)
                    .WithFailure(HttpStatusCode.BadRequest)
                    .Build();
            }

            var productsResult = ConvertImportedProducts(importedProducts.Products);
            if (!productsResult.IsSuccessful)
            {
                return resultBuilder.FromResult(productsResult).Build();
            }

            try
            {
                await _repository.AddUpdateProductsAsync(productsResult.Response);
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

        private async Task<ImportedProducts> DeserializeProductsStream(Stream productsFileStream)
        {
            _logger.LogTrace("Starting to deserialize the uploaded json");

            if (productsFileStream.CanSeek)
            {
                productsFileStream.Seek(0, SeekOrigin.Begin);
            }

            using var streamReader = new StreamReader(productsFileStream);
            var productsJson = await streamReader.ReadToEndAsync();
            if (productsJson == string.Empty)
            {
                _logger.LogError("Empty json file detected");
                throw new WarehouseException("Json file is empty");
            }

            ImportedProducts parsedJson;
            try
            {
                parsedJson = JsonConvert.DeserializeObject<ImportedProducts>(productsJson);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not deserialize the json file", e);
                throw new WarehouseException(e.Message);
            }

            if (parsedJson.Products == null)
            {
                _logger.LogError("Parsed json has no products property");
                throw new WarehouseException("Json file does not contain products data");
            }

            return parsedJson;
        }

        private Result<List<Product>> ConvertImportedProducts(List<ImportedProduct> jsonProducts)
        {
            var resultBuilder = new ResultBuilder<List<Product>>();
            var importedProducts = new List<Product>();

            for (var i = 0; i < jsonProducts.Count; i++)
            {
                var jsonProduct = jsonProducts[i];

                if (string.IsNullOrWhiteSpace(jsonProduct.Name))
                {
                    _logger.LogError("Empty product name detected");
                    resultBuilder.WithError("Article product cannot be empty", new Dictionary<string, string> { { "productIndex", i.ToString() } });
                }

                var importedProduct = new Product { Name = jsonProduct.Name };

                if (jsonProduct.Articles == null || jsonProduct.Articles.Count == 0)
                {
                    _logger.LogError("Imported product doesn't have articles");
                    resultBuilder.WithError("Product must contain articles", new Dictionary<string, string> { { "productIndex", i.ToString() } });
                }
                else
                {
                    foreach (var jsonProductArticle in jsonProduct.Articles)
                    {

                        if (!int.TryParse(jsonProductArticle.ArticleId, out var articleId))
                        {
                            _logger.LogError($"Invalid article id detected: {jsonProductArticle.ArticleId}");
                            resultBuilder.WithError($"Invalid article id: {jsonProductArticle.ArticleId}", new Dictionary<string, string> { { "productIndex", i.ToString() } });
                        }

                        if (!int.TryParse(jsonProductArticle.Amount, out var amount) || amount < 1)
                        {
                            _logger.LogError($"Invalid article amount detected: {jsonProductArticle.Amount}");
                            resultBuilder.WithError($"Invalid article amount: {jsonProductArticle.Amount}", new Dictionary<string, string> { { "productIndex", i.ToString() } });
                        }

                        importedProduct.ProductArticles.Add(new ProductArticle{ArticleId = articleId, AmountOfArticles = amount});
                    }
                }

                importedProducts.Add(importedProduct);
            }

            if (resultBuilder.HasErrors())
            {
                return resultBuilder
                    .WithFailure(HttpStatusCode.BadRequest)
                    .Build();
            }

            return resultBuilder
                .WithData(importedProducts)
                .WithSuccess()
                .Build();
        }
    }
}

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
    internal class ImportInventoryHandler : IImportInventoryHandler
    {
        private readonly IWarehouseRepository _repository;
        private readonly ILogger<ImportInventoryHandler> _logger;

        public ImportInventoryHandler(
            IWarehouseRepository repository,
            ILogger<ImportInventoryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result> ImportAsync(Stream inventoryFileStream)
        {
            _logger.LogTrace("Starting to import articles");

            var resultBuilder = new ResultBuilder();


            var importedInventoryResult = await DeserializeInventoryStream(inventoryFileStream);
            if (!importedInventoryResult.IsSuccessful)
            {
                return resultBuilder
                    .FromResult(importedInventoryResult)
                    .Build();
            }

            var getArticlesResult = ConvertImportedArticles(importedInventoryResult.Response.Articles);
            if (!getArticlesResult.IsSuccessful)
            {
                return resultBuilder
                    .FromResult(getArticlesResult)
                    .Build();
            }

            try
            {
                await _repository.AddUpdateArticlesAsync(getArticlesResult.Response);
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

        private async Task<Result<ImportedInventory>> DeserializeInventoryStream(Stream inventoryFileStream)
        {
            _logger.LogTrace("Starting to deserialize the uploaded json");

            var resultBuilder = new ResultBuilder<ImportedInventory>();

            if (inventoryFileStream.CanSeek)
            {
                inventoryFileStream.Seek(0, SeekOrigin.Begin);
            }

            using var streamReader = new StreamReader(inventoryFileStream);
            var inventoryJson = await streamReader.ReadToEndAsync();
            if (inventoryJson == string.Empty)
            {
                _logger.LogError("Empty json file detected");
                return resultBuilder
                    .WithFailure(HttpStatusCode.BadRequest)
                    .WithError("Json file is empty")
                    .Build();
            }

            ImportedInventory parsedJson;
            try
            {
                parsedJson = JsonConvert.DeserializeObject<ImportedInventory>(inventoryJson);
            }
            catch (Exception e)
            {
                _logger.LogError("Could not deserialize the json file", e);
                return resultBuilder
                    .WithFailure(HttpStatusCode.BadRequest)
                    .WithError(e.Message)
                    .Build();
            }

            if (parsedJson?.Articles == null)
            {
                _logger.LogError("Parsed json has no articles property");
                return resultBuilder
                    .WithFailure(HttpStatusCode.BadRequest)
                    .WithError("Json file does not contain inventory data")
                    .Build();
            }

            return resultBuilder
                .WithSuccess()
                .WithData(parsedJson)
                .Build();
        }

        private Result<List<Article>> ConvertImportedArticles(List<ImportedArticle> importedArticles)
        {
            var resultBuilder = new ResultBuilder<List<Article>>();
            var articles = new List<Article>();

            for (var i = 0; i < importedArticles.Count; i++)
            {
                var jsonArticle = importedArticles[i];
                if (!int.TryParse(jsonArticle.ArticleId, out var articleId))
                {
                    _logger.LogError($"Invalid article id detected: {jsonArticle.ArticleId}");
                    resultBuilder.WithError($"Invalid article id: {jsonArticle.ArticleId}", new Dictionary<string, string> { { "articleIndex", i.ToString() } });
                }

                if (string.IsNullOrWhiteSpace(jsonArticle.Name))
                {
                    _logger.LogError("Empty article name detected");
                    resultBuilder.WithError("Article name cannot be empty", new Dictionary<string, string> { { "articleIndex", i.ToString() } });
                }

                if (!int.TryParse(jsonArticle.StockQuantity, out var stockQuantity) || stockQuantity < 0)
                {
                    _logger.LogError($"Invalid article stock quantity: {jsonArticle.StockQuantity}");
                    resultBuilder.WithError($"Invalid article stock: {jsonArticle.StockQuantity}", new Dictionary<string, string> { { "articleIndex", i.ToString() } });
                }

                articles.Add(new Article { ArticleId = articleId, Name = jsonArticle.Name, StockQuantity = stockQuantity });
            }

            if (resultBuilder.HasErrors())
            {
                return resultBuilder
                    .WithFailure(HttpStatusCode.BadRequest)
                    .Build();
            }

            return resultBuilder
                .WithData(articles)
                .WithSuccess()
                .Build();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers
{
    internal class AddUpdateArticlesHandler : IAddUpdateArticlesHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<AddUpdateArticlesHandler> _logger;

        public AddUpdateArticlesHandler(
            WarehouseDbContext dbContext,
            ILogger<AddUpdateArticlesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddUpdateArticlesAsync(List<Article> articles)
        {
            _logger.LogTrace("Starting AddUpdateArticlesAsync");

            try
            {
                foreach (var article in articles)
                {
                    var existingArticle = await _dbContext.Articles.FindAsync(article.ArticleId);
                    if (existingArticle != null)
                    {
                        existingArticle.Name = article.Name;
                        existingArticle.StockQuantity = article.StockQuantity;
                    }
                    else
                    {
                        await _dbContext.Articles.AddAsync(article);
                    }
                }

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("An unexpected error occured while saving the articles");
            }

        }
    }
}

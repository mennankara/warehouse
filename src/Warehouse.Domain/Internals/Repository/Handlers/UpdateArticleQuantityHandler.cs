using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;

namespace Warehouse.Domain.Internals.Repository.Handlers
{
    internal class UpdateArticleQuantityHandler : IUpdateArticleQuantityHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<UpdateArticleQuantityHandler> _logger;

        public UpdateArticleQuantityHandler(
            WarehouseDbContext dbContext,
            ILogger<UpdateArticleQuantityHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task UpdateQuantityAsync(int articleId, int amount)
        {
            _logger.LogTrace("Starting UpdateQuantityAsync");

            try
            {
                var article = await _dbContext.Articles.FindAsync(articleId);
                if (article == null)
                {
                    _logger.LogError($"Non-existing article: {articleId}");
                    throw new WarehouseException("Product component not found");
                }

                article.StockQuantity += amount;

                await _dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("An unexpected error occured while updating the article");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers
{
    internal class GetArticlesHandler : IGetArticlesHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<GetArticlesHandler> _logger;

        public GetArticlesHandler(
            WarehouseDbContext dbContext,
            ILogger<GetArticlesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Article>> GetArticlesAsync(List<int> articleIds = null)
        {
            try
            {
                return articleIds == null
                    ? await _dbContext.Articles.ToListAsync()
                    : await _dbContext.Articles.Where(a => articleIds.Contains(a.ArticleId)).ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("Could not retrive the articles");
            }
        }
    }
}

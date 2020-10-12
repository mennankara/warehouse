using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers
{
    internal class GetProductsHandler:IGetProductsHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<GetProductsHandler> _logger;

        public GetProductsHandler(WarehouseDbContext dbContext, ILogger<GetProductsHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            _logger.LogTrace("Starting AddUpdateArticlesAsync");

            try
            {
                return await _dbContext.Products.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("Could not retrive the products");
            }
        }
    }
}

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
    internal class AddUpdateProductsHandler : IAddUpdateProductsHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<AddUpdateProductsHandler> _logger;

        public AddUpdateProductsHandler(
            WarehouseDbContext dbContext,
            ILogger<AddUpdateProductsHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddUpdateProductsAsync(List<Product> products)
        {
            _logger.LogTrace("Starting AddUpdateProducts");

            try
            {
                foreach (var product in products)
                {
                    var existingProduct = await _dbContext.Products.FindAsync(product.Name);
                    if (existingProduct != null)
                    {
                        existingProduct.Name = product.Name;
                        existingProduct.ProductArticles = product.ProductArticles;
                    }
                    else
                    {
                        await _dbContext.Products.AddAsync(product);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("An unexpected error occured while saving the Products");
            }

        }
    }
}

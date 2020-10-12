using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Warehouse.Common;
using Warehouse.Domain.Internals.Repository.DataAccess;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers
{
    internal class GetProductHandler : IGetProductHandler
    {
        private readonly WarehouseDbContext _dbContext;
        private readonly ILogger<GetProductHandler> _logger;

        public GetProductHandler(
            WarehouseDbContext dbContext,
            ILogger<GetProductHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Product> GetProductAsync(string name)
        {
            try
            {
                return await _dbContext.Products.FindAsync(name);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new WarehouseException("Could not retrieve the product");
            }
        }
    }
}
    

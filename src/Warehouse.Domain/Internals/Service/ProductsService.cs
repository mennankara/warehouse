using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;
using Warehouse.Domain.Models.Responses;

namespace Warehouse.Domain.Internals.Service
{
    internal class ProductsService: IProductsService
    {
        private readonly IImportProductsHandler _importProductsHandler;
        private readonly IGetProductStocksHandler _getProductStocksHandler;
        private readonly IOrderProductHandler _orderProductHandler;

        public ProductsService(
            IImportProductsHandler importProductsHandler,
            IGetProductStocksHandler getProductStocksHandler,
            IOrderProductHandler orderProductHandler)
        {
            _importProductsHandler = importProductsHandler;
            _getProductStocksHandler = getProductStocksHandler;
            _orderProductHandler = orderProductHandler;
        }

        public async Task<Result> ImportAsync(Stream productsFileStream)
        {
            return await _importProductsHandler.ImportAsync(productsFileStream);
        }

        public async Task<Result<List<ProductStockResponse>>> GetProductStocksAsync()
        {
            return await _getProductStocksHandler.GetProductStocksAsync();
        }

        public async Task<Result> OrderProductAsync(string name)
        {
            return await _orderProductHandler.OrderProductAsync(name);
        }
    }
}

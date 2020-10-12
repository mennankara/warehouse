using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Common;
using Warehouse.Domain.Models.Responses;

namespace Warehouse.Domain.Internals.Service.Handlers.Abstractions
{
    internal interface IGetProductStocksHandler
    {
        Task<Result<List<ProductStockResponse>>> GetProductStocksAsync();
    }
}
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;
using Warehouse.Domain.Models.Responses;

namespace Warehouse.Domain
{
    public interface IProductsService
    {
        Task<Result> ImportAsync(Stream productsFileStream);
        Task<Result<List<ProductStockResponse>>> GetProductStocksAsync();
        Task<Result> OrderProductAsync(string name);
    }
}
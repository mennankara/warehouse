using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;

namespace Warehouse.Domain.Internals.Service.Handlers.Abstractions
{
    internal interface IImportProductsHandler
    {
        Task<Result> ImportAsync(Stream productsFileStream);
    }
}
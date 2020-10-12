using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;

namespace Warehouse.Domain.Internals.Service.Handlers.Abstractions
{
    internal interface IImportInventoryHandler
    {
        Task<Result> ImportAsync(Stream inventoryFileStream);
    }
}
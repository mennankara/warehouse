using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;
using Warehouse.Domain.Internals.Service.Handlers.Abstractions;

namespace Warehouse.Domain.Internals.Service
{
    internal class InventoryService : IInventoryService
    {
        private readonly IImportInventoryHandler _importInventoryHandler;

        public InventoryService(IImportInventoryHandler importInventoryHandler)
        {
            _importInventoryHandler = importInventoryHandler;
        }

        public async Task<Result> ImportAsync(Stream inventoryFileStream)
        {
            return await _importInventoryHandler.ImportAsync(inventoryFileStream);
        }
    }
}

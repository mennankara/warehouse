using System.IO;
using System.Threading.Tasks;
using Warehouse.Common;

namespace Warehouse.Domain
{
    public interface IInventoryService
    {
        Task<Result> ImportAsync(Stream inventoryFileStream);
    }
}
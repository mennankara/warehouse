using System.Threading.Tasks;
using Warehouse.Common;

namespace Warehouse.Domain.Internals.Service.Handlers.Abstractions
{
    internal interface IOrderProductHandler
    {
        Task<Result> OrderProductAsync(string name);
    }
}
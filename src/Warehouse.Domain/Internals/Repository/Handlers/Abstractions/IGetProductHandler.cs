using System.Threading.Tasks;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers.Abstractions
{
    internal interface IGetProductHandler
    {
        Task<Product> GetProductAsync(string name);
    }
}
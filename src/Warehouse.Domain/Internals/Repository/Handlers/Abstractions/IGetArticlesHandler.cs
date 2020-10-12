using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository.Handlers.Abstractions
{
    internal interface IGetArticlesHandler
    {
        Task<List<Article>> GetArticlesAsync(List<int> articleIds = null);
    }
}
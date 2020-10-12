using System.Threading.Tasks;

namespace Warehouse.Domain.Internals.Repository.Handlers.Abstractions
{
    internal interface IUpdateArticleQuantityHandler
    {
        Task UpdateQuantityAsync(int articleId, int amount);
    }
}
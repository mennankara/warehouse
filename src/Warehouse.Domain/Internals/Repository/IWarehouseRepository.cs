using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository
{
    internal interface IWarehouseRepository
    {
        Task AddUpdateArticlesAsync(List<Article> articles);
        Task AddUpdateProductsAsync(List<Product> products);
        Task DecreaseArticleQuantityAsync(int articleId, int amount);
        Task IncreaseArticleQuantityAsync(int articleId, int amount);
        Task<List<Article>> GetArticlesAsync(List<int> articleIds = null);
        Task<Product> GetProductAsync(string name);
        Task<List<Product>> GetProductsAsync();
    }
}
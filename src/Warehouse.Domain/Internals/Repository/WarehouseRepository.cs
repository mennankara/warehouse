using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain.Internals.Repository.Handlers.Abstractions;
using Warehouse.Domain.Internals.Repository.Models;

namespace Warehouse.Domain.Internals.Repository
{
    internal class WarehouseRepository : IWarehouseRepository
    {
        private readonly IUpdateArticleQuantityHandler _updateArticleQuantityHandler;
        private readonly IGetArticlesHandler _getArticlesHandler;
        private readonly IAddUpdateArticlesHandler _addUpdateArticlesHandler;
        private readonly IGetProductHandler _getProductHandler;
        private readonly IGetProductsHandler _getProductsHandler;
        private readonly IAddUpdateProductsHandler _addUpdateProductsHandler;

        public WarehouseRepository(
            IUpdateArticleQuantityHandler updateArticleQuantityHandler,
            IGetArticlesHandler getArticlesHandler,
            IAddUpdateArticlesHandler addUpdateArticlesHandler,
            IGetProductHandler getProductHandler,
            IGetProductsHandler getProductsHandler,
            IAddUpdateProductsHandler addUpdateProductsHandler)
        {
            _updateArticleQuantityHandler = updateArticleQuantityHandler;
            _getArticlesHandler = getArticlesHandler;
            _addUpdateArticlesHandler = addUpdateArticlesHandler;
            _getProductHandler = getProductHandler;
            _getProductsHandler = getProductsHandler;
            _addUpdateProductsHandler = addUpdateProductsHandler;
        }

        public async Task IncreaseArticleQuantityAsync(int articleId, int amount)
        {
            await _updateArticleQuantityHandler.UpdateQuantityAsync(articleId, amount);
        }

        public async Task DecreaseArticleQuantityAsync(int articleId, int amount)
        {
            await _updateArticleQuantityHandler.UpdateQuantityAsync(articleId, amount * -1);
        }

        public async Task<List<Article>> GetArticlesAsync(List<int> articleIds = null)
        {
            return await _getArticlesHandler.GetArticlesAsync();
        }

        public async Task AddUpdateArticlesAsync(List<Article> articles)
        {
            await _addUpdateArticlesHandler.AddUpdateArticlesAsync(articles);
        }

        public async Task<Product> GetProductAsync(string name)
        {
            return await _getProductHandler.GetProductAsync(name);
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _getProductsHandler.GetProductsAsync();
        }

        public async Task AddUpdateProductsAsync(List<Product> products)
        {
            await _addUpdateProductsHandler.AddUpdateProductsAsync(products);
        }

    }
}

using System.Collections.Generic;

namespace Warehouse.Domain.Internals.Repository.Models
{
    internal class Product
    {
        public string Name { get; set; }
        public List<ProductArticle> ProductArticles { get; set; } = new List<ProductArticle>();
    }
}
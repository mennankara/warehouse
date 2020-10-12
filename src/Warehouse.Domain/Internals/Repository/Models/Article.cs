namespace Warehouse.Domain.Internals.Repository.Models
{
    internal class Article
    {
        public int ArticleId { get; set; }

        public string Name { get; set; }

        public int StockQuantity { get; set; }
    }
}

using Newtonsoft.Json;

namespace Warehouse.Domain.Internals.Service.Models
{
    internal class ImportedArticle
    {
        [JsonProperty("art_id")]
        public string ArticleId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("stock")]
        public string StockQuantity { get; set; }
    }
}

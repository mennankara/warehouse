using Newtonsoft.Json;

namespace Warehouse.Domain.Internals.Service.Models
{
    internal class ImportedProductArticle
    {
        [JsonProperty("art_id")]
        public string ArticleId { get; set; }

        [JsonProperty("amount_of")]
        public string Amount { get; set; }
    }
}
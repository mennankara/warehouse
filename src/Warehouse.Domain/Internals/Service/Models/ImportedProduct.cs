using System.Collections.Generic;
using Newtonsoft.Json;

namespace Warehouse.Domain.Internals.Service.Models
{
    internal class ImportedProduct
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("contain_articles")]
        public List<ImportedProductArticle> Articles { get; set; }
    }
}

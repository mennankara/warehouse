using System.Collections.Generic;
using Newtonsoft.Json;

namespace Warehouse.Domain.Internals.Service.Models
{
    internal class ImportedProducts
    {
        [JsonProperty("products")]
        public List<ImportedProduct> Products { get; set; }
    }
}
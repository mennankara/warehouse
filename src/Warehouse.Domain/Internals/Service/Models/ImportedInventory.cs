using System.Collections.Generic;
using Newtonsoft.Json;

namespace Warehouse.Domain.Internals.Service.Models
{
    internal class ImportedInventory
    {
        [JsonProperty("inventory")]
        public List<ImportedArticle> Articles { get; set; }
    }
}
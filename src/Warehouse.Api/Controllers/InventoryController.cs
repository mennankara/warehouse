using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Warehouse.Common;
using Warehouse.Domain;

namespace Warehouse.Api.Controllers
{
    [ApiController]
    [Route("inventory")]
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpPost("import")]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "Import inventory")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(List<Error>))]
        public async Task<IActionResult> Import(IFormFile inventoryJsonFile)
        {
            if (inventoryJsonFile == null)
            {
                return BadRequest(new List<Error> { new Error("inventoryJsonFile is required") });
            }

            if (!inventoryJsonFile.FileName.ToLowerInvariant().EndsWith(".json"))
            {
                return BadRequest(new List<Error> { new Error("inventoryJsonFile should have the extension .json") });
            }

            var result = await _inventoryService.ImportAsync(inventoryJsonFile.OpenReadStream());

            return result.IsSuccessful
                ? (IActionResult) NoContent()
                : StatusCode((int) result.FailureStatusCode, result.Errors);
        }
    }
}
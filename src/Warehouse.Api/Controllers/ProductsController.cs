using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Warehouse.Common;
using Warehouse.Domain;
using Warehouse.Domain.Models.Responses;

namespace Warehouse.Api.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            _productsService = productsService;
        }

        [HttpPost("import")]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "Import products")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(void))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(List<Error>))]
        public async Task<IActionResult> Import(IFormFile productsJsonFile)
        {
            if (productsJsonFile == null)
            {
                return BadRequest(new List<Error> { new Error("productsJsonFile is required") });
            }

            if (!productsJsonFile.FileName.ToLowerInvariant().EndsWith(".json"))
            {
                return BadRequest(new List<Error> { new Error("productsJsonFile should have the extension .json") });
            }

            var result = await _productsService.ImportAsync(productsJsonFile.OpenReadStream());
            return result.IsSuccessful
                ? (IActionResult) NoContent()
                : StatusCode((int) result.FailureStatusCode, result.Errors);
        }

        [HttpGet("stocks")]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "Get products with their stock quantities")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(List<ProductStockResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(List<Error>))]
        public async Task<IActionResult> GetProductStocks()
        {
            var result = await _productsService.GetProductStocksAsync();
            return result.IsSuccessful
                ? Ok(result.Response)
                : StatusCode((int)result.FailureStatusCode, result.Errors);
        }

        [HttpPost("order")]
        [Produces("application/json")]
        [SwaggerOperation(OperationId = "Order a product")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(List<ProductStockResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(List<Error>))]
        public async Task<IActionResult> OrderProduct(string name)
        {
            var result = await _productsService.OrderProductAsync(name);
            return result.IsSuccessful
                ? (IActionResult) NoContent()
                : StatusCode((int)result.FailureStatusCode, result.Errors);
        }
    }
}
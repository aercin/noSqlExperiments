using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.v1_1
{
    [ApiController] 
    [Route("api/v{version:apiVersion}")]
    [ApiVersion("1.1")]
    public class ProductController : ControllerBase
    {

        [MapToApiVersion("1.1")]
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> QueryAllProducts()
        {
            return Ok(await Task.FromResult("version is 1.1"));
        }
    }
}
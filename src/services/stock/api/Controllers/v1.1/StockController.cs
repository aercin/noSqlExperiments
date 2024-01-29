using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.v1_1
{
    [ApiController]
    [Route("api/v{version:apiVersion}")]
    [ApiVersion("1.1")]
    public class StockController : ControllerBase
    {
        [MapToApiVersion("1.1")]
        [HttpGet]
        [Route("stocks")]
        public async Task<IActionResult> QueryAllStocks()
        {
            return Ok(await Task.FromResult("version is 1.1"));
        }
    }
}

using api.Models;
using application.Features.Commands;
using application.Features.Queries;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers.v1_0
{
    [ApiController]
    [Route("api/v{version:apiVersion}")]
    [ApiVersion("1.0")]
    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StockController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("stocks/{stockId}/products")]
        public async Task<IActionResult> AddProductToStockAsync(Guid stockId, AddProductToStockRequest request)
        {
            return Ok(await this._mediator.Send(new AddProductToStock.Command
            {
                StockId = stockId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            }));
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("stocks")]
        public async Task<IActionResult> QueryStockAsync([FromQuery] GetStock.Query request)
        {
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("stocks/{stockId}/products/{productId}")]
        public async Task<IActionResult> QueryProductInStockAsync([FromQuery] GetSpecificProductInStock.Query request)
        {
            return Ok(await this._mediator.Send(request));
        }
    }
}

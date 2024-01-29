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
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("orders")]
        public async Task<IActionResult> PlaceOrderAsync(OrderPlace.Command request)
        {
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("orders")]
        public async Task<IActionResult> QueryOrdersAsync([FromQuery] GetOrders.Query request)
        {
            return Ok(await this._mediator.Send(request));
        }
    }
}

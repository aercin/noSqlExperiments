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
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BasketController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("baskets/{userId}/products")]
        public async Task<IActionResult> AddProductToBasketAsync(Guid userId, AddProductToBasket.Command request)
        {
            request.UserId = userId;
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpDelete]
        [Route("baskets/{userId}/products")]
        public async Task<IActionResult> RemoveProductFromBasketAsync(Guid userId, RemoveProductFromBasket.Command request)
        {
            request.UserId = userId;
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("baskets")]
        public async Task<IActionResult> QueryBasketAsync([FromQuery] GetBasket.Query request)
        {
            return Ok(await this._mediator.Send(request));
        }
    }
}
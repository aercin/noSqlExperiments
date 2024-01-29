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
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        [Route("products")]
        public async Task<IActionResult> CreateProductAsync(CreateProduct.Command request)
        {
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpPut]
        [Route("products/{id}")]
        public async Task<IActionResult> UpdateProductAsync(Guid id, UpdateProduct.Command request)
        {
            request.ProductId = id;
            return Ok(await this._mediator.Send(request));
        }

        [MapToApiVersion("1.0")]
        [HttpDelete]
        [Route("products/{id}")]
        public async Task<IActionResult> DeleteProductAsync(Guid id)
        {
            return Ok(await this._mediator.Send(new DeleteProduct.Command { ProductId = id }));
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        [Route("products")]
        public async Task<IActionResult> QueryProductsAsync([FromQuery]GetProducts.Query request)
        {
            return Ok(await this._mediator.Send(request));
        }
    }
}
using application.Abstractions;
using core_application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace application.Features.Commands
{
    public static class UpdateProduct
    {
        #region Command
        public class Command : IRequest<Result>
        {
            [JsonIgnore]
            public Guid ProductId { get; set; }
            public decimal Price { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IProductRepository _productRepository;
            private readonly ILogger _logger;
            public CommandHandler(IProductRepository productRepository,
                                  ILogger<CommandHandler> logger)
            {
                this._productRepository = productRepository;
                this._logger = logger;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                this._logger.LogInformation("Sistemde ürün fiyat güncellemesi yapılmaktadır");

                var relatedProduct = await this._productRepository.GetAsync(request.ProductId);
                if (relatedProduct == null)
                {
                    return Result.Failure(new List<string> { "İşlem yapılmak istenen ürün sistemde bulunmamaktadır" });
                }

                relatedProduct.Price = request.Price;

                var updateResult = await this._productRepository.UpdateAsync(relatedProduct);

                return updateResult ? Result.Success() : Result.Failure(new List<string>());
            }
        }
        #endregion

        #region Data Validation
        public class UpdateProductValidator : AbstractValidator<Command>
        {
            public UpdateProductValidator()
            {
                RuleFor(c => c.ProductId).NotEmpty().WithMessage("ProductId alanı boş geçilemez");
                RuleFor(c => c.Price).GreaterThan(0).WithMessage("Price alanı boş geçilemez");
            }
        }
        #endregion
    }
}

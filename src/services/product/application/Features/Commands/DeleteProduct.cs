using application.Abstractions;
using core_application.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace application.Features.Commands
{
    public static class DeleteProduct
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public Guid ProductId { get; set; }
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
                this._logger.LogInformation("Sistemden tanımlı ürün silinmektedir");

                var relatedProduct = await this._productRepository.GetAsync(request.ProductId);
                if (relatedProduct == null)
                {
                    return Result.Failure(new List<string> { "İşlem yapılmak istenen ürün sistemde bulunmamaktadır" });
                }

                var deleteResult = await this._productRepository.DeleteAsync(request.ProductId, relatedProduct.Cas);

                return deleteResult ? Result.Success() : Result.Failure(new List<string>());
            }
        }
        #endregion

        #region Data Validation
        public class DeleteProductValidator : AbstractValidator<Command>
        {
            public DeleteProductValidator()
            {
                RuleFor(c => c.ProductId).NotEmpty().WithMessage("ProductId alanı boş geçilemez"); 
            }
        }
        #endregion
    }
}
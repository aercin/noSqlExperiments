using application.Abstractions;
using core_application.Common;
using domain.Entities;
using FluentValidation;
using MediatR;

namespace application.Features.Commands
{
    public static class AddProductToStock
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public Guid StockId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IUnitOfWork _uow;
            public CommandHandler(IUnitOfWork uow)
            {
                this._uow = uow;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var stock = await this._uow.StockRepo.FindOneAsync(x => x.Id == request.StockId.ToString());
                if (stock == null)
                {
                    stock = new Stock
                    {
                        Id = request.StockId.ToString(),
                        Products = new List<Product>
                        {
                            new Product
                            {
                                Id = request.ProductId.ToString(),
                                Quantity = request.Quantity
                            }
                        }
                    };

                    await this._uow.StockRepo.InsertOneAsync(stock);
                }
                else
                {
                    var product = stock.Products.SingleOrDefault(x => x.Id == request.ProductId.ToString());
                    if (product == null)
                    {
                        stock.Products.Add(new Product
                        {
                            Id = request.ProductId.ToString(),
                            Quantity = request.Quantity
                        });
                    }
                    else
                    {
                        product.Quantity += request.Quantity;
                    }

                    await this._uow.StockRepo.ReplaceOneAsync(stock);
                }

                return Result<string>.Success(stock.Id);
            }
        }
        #endregion

        #region Data Validation
        public class AddProductToStockValidator : AbstractValidator<Command>
        {
            public AddProductToStockValidator()
            {
                RuleFor(c => c.StockId).NotEmpty().WithMessage("StockId alanı boş geçilemez");
                RuleFor(c => c.ProductId).NotEmpty().WithMessage("ProductId alanı boş geçilemez");
                RuleFor(c => c.Quantity).NotEmpty().WithMessage("Quantity alanı boş geçilemez");
            }
        }
        #endregion 
    }
}

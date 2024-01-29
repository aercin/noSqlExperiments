using application.Abstractions;
using core_application.Common;
using domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace application.Features.Commands
{
    public static class RemoveProductFromBasket
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public Guid UserId { get; set; }
            public Guid ProductId { get; set; }
            public int Quantity { get; set; }
        }

        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IRedisRepository _redisRepository; 
            private readonly ILogger _logger;
            private readonly IConfiguration _config;
            public CommandHandler(IRedisRepository redisRepository, 
                                  ILogger<CommandHandler> logger,
                                  IConfiguration config)
            {
                this._redisRepository = redisRepository; 
                this._logger = logger;
                this._config = config;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                this._logger.LogInformation("Sepetten ürün çıkartılmaktadır");

                if (!await this._redisRepository.IsKeyExistAsync(request.UserId.ToString()))
                {
                    return Result.Failure(new List<string> { "Kullanıcıya tanımlı bir sepet bulunmamaktadır" });
                }

                var userBasket = await this._redisRepository.GetAsync<Basket>(request.UserId.ToString());

                var basketItem = userBasket.Items.SingleOrDefault(x => x.ProductId == request.ProductId);

                if (basketItem == null)
                {
                    return Result.Failure(new List<string> { "Çıkartılmak istenen ürün sepette bulunmamaktadır" });
                }

                if (basketItem.Quantity < request.Quantity)
                {
                    return Result.Failure(new List<string> { "Çıkartılmak istenen ürün için belirtilen sayıda sepette ürün bulunmamaktadır" });
                }

                if (basketItem.Quantity == request.Quantity)
                {
                    userBasket.Items.Remove(basketItem);
                }
                else
                {
                    basketItem.Quantity -= request.Quantity;
                }

                await this._redisRepository.SetAsync(request.UserId.ToString(), userBasket, TimeSpan.FromMinutes(this._config.GetValue<int>("Basket:Expiration")));

                return Result.Success();
            }
        }
        #endregion
          
        #region Data Validation

        public class RemoveProductFromBasketValidator : AbstractValidator<Command>
        {
            public RemoveProductFromBasketValidator()
            {
                RuleFor(c => c.UserId).NotEmpty().WithMessage("UserId alanı boş geçilemez");
                RuleFor(c => c.ProductId).NotEmpty().WithMessage("ProductId alanı boş geçilemez");
                RuleFor(c => c.Quantity).GreaterThan(0).WithMessage("Quantity alanı boş geçilemez");
            }
        }
        #endregion 
    }
}

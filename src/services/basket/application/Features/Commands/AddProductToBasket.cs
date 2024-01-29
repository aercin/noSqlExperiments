using application.Abstractions;
using AutoMapper;
using core_application.Common;
using domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace application.Features.Commands
{
    public static class AddProductToBasket
    {
        #region Command
        public class Command : IRequest<Result>
        {
            [JsonIgnore]
            public Guid UserId { get; set; }
            public Guid ProductId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IRedisRepository _redisRepository;
            private readonly IMapper _mapper;
            private readonly ILogger _logger;
            private readonly IConfiguration _config;
            public CommandHandler(IRedisRepository redisRepository,
                                  IMapper mapper,
                                  ILogger<CommandHandler> logger,
                                  IConfiguration config)
            {
                this._redisRepository = redisRepository;
                this._mapper = mapper;
                this._logger = logger;
                this._config = config;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                this._logger.LogInformation("Sepete yeni bir ürün eklenmektedir");

                Basket userBasket;

                if (await this._redisRepository.IsKeyExistAsync(request.UserId.ToString()))
                {
                    userBasket = await this._redisRepository.GetAsync<Basket>(request.UserId.ToString());
                }
                else
                {
                    userBasket = new Basket { UserId = request.UserId };
                }

                if (userBasket.Items.Any(x => x.ProductId == request.ProductId))
                {
                    var basketItem = userBasket.Items.Single(x => x.ProductId == request.ProductId);
                    basketItem.Quantity += request.Quantity;
                    basketItem.Price = request.Price; 
                }
                else
                {
                    userBasket.Items.Add(this._mapper.Map<BasketItem>(request));
                }
                 
                await this._redisRepository.SetAsync(request.UserId.ToString(), userBasket, TimeSpan.FromMinutes(this._config.GetValue<int>("Redis:Expiration")));

                return Result.Success();
            }
        }
        #endregion

        #region Mapping Profile
        public class AddProductToBasketProfile : Profile
        {
            public AddProductToBasketProfile()
            {
                CreateMap<Command, BasketItem>();
            }
        }
        #endregion

        #region Data Validation
        public class AddProductToBasketValidator : AbstractValidator<Command>
        {
            public AddProductToBasketValidator()
            {
                RuleFor(c => c.UserId).NotEmpty().WithMessage("UserId alanı boş geçilemez");
                RuleFor(c => c.ProductId).NotEmpty().WithMessage("ProductId alanı boş geçilemez");
                RuleFor(c => c.Quantity).GreaterThan(0).WithMessage("Quantity alanı boş geçilemez");
            }
        }
        #endregion
    }
}

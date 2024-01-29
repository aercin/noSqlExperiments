using application.Abstractions;
using AutoMapper;
using core_application.Common;
using core_domain.Entities;
using core_messages.IntegrationEvents;
using domain.Entities;
using domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace application.Features.Commands
{
    public static class OrderPlace
    {
        #region Command
        public class Command : IRequest<Result>
        {
            public string UserId { get; set; }
        }
        #endregion

        #region Command Handler
        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IMapper _mapper;
            private readonly IConfiguration _configuration;
            private readonly IHttpClientFactory _httpClientFactory;
            private readonly IUnitOfWork _uow;
            public CommandHandler(IUnitOfWork uow,
                                  IHttpClientFactory httpClientFactory,
                                  IConfiguration configuration,
                                  IMapper mapper)
            {
                this._uow = uow;
                this._httpClientFactory = httpClientFactory;
                this._configuration = configuration;
                this._mapper = mapper;
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                //Müşterinin hangi sepete istinaden sipariş verdiğini öğreniyoruz.
                var httpClient = this._httpClientFactory.CreateClient();
                var resBasket = await httpClient.GetAsync(string.Format(this._configuration.GetValue<string>("Integration:Sync:BasketServiceUrl"), request.UserId), cancellationToken);
                resBasket.EnsureSuccessStatusCode();

                var jsonBasketDetail = await resBasket.Content.ReadAsStringAsync();

                BasketResult resBasketService = JsonSerializer.Deserialize<BasketResult>(jsonBasketDetail, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
           
                if (resBasketService?.Data == null || resBasketService.Data.Count == 0)
                    return Result.Failure(new List<string> { "Siparişe konu bir sepet bulunamadı" });

                try
                {
                    var session = await this._uow.StartNewSessionAsync();

                    var newOrderId = Guid.NewGuid();

                    await this._uow.OrderRepo.InsertOneAsync(new Order
                    {
                        Id = newOrderId.ToString(),
                        UserId = request.UserId,
                        Status = OrderStatus.Suspend,
                        Products = this._mapper.Map<List<Product>>(resBasketService.Data)
                    }, session);

                    await this._uow.OutboxMessageRepo.InsertOneAsync(new OutboxMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        ServiceName = "order",
                        Topic = this._configuration.GetValue<string>("Integration:Async:Produce:Topic"),
                        Message = JsonSerializer.Serialize(new OrderPlacedEvent
                        {
                            OrderId = newOrderId,
                            Items = this._mapper.Map<List<OrderPlacedEvent.OrderItem>>(resBasketService.Data)
                        }),
                        CreatedOn = DateTime.Now
                    }, session);

                    await this._uow.CommitAsync();
                }
                catch(Exception ex)
                {
                    await this._uow.RollbackAsync();
                    throw;
                }

                return Result.Success();
            }
        }
        #endregion

        #region Data Validation
        public class OrderPlaceValidator : AbstractValidator<Command>
        {
            public OrderPlaceValidator()
            {
                RuleFor(c => c.UserId).NotEmpty().WithMessage("UserId alanı boş geçilemez");
            }
        }
        #endregion

        #region Mapping Profile
        public class OrderPlaceProfile : Profile
        {
            public OrderPlaceProfile()
            {
                CreateMap<BasketItem, Product>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId.ToString()));
                CreateMap<BasketItem, OrderPlacedEvent.OrderItem>();
            }
        }
        #endregion

        #region Dtos

        public class BasketResult
        {
            public List<BasketItem> Data { get; set; }
        }
        public class BasketItem
        {
            public Guid ProductId { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        #endregion
    }
}

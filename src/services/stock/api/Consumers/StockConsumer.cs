using core_application.Abstractions;
using core_messages.IntegrationEvents;
using core_messages;
using MediatR;
using System.Text.Json;
using application.Features.Commands;
using AutoMapper;

namespace api.Consumers
{
    public class StockConsumer : BackgroundService
    {
        private readonly IEventListener _eventListener;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public StockConsumer(IEventListener eventListener,
                             IConfiguration configuration,
                             IServiceProvider serviceProvider)
        {
            this._eventListener = eventListener;
            this._configuration = configuration;
            this._serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(1000);

            await this._eventListener.ConsumeEvent(this._configuration.GetValue<string>("Integration:Async:Consume:Topic"), async (message) =>
            {
                var integrationEventBase = JsonSerializer.Deserialize<IntegrationEventBase>(message);

                if (integrationEventBase.EventType == typeof(OrderPlacedEvent).FullName)
                {
                    var objOrderPlacedEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(message);
                    using (var scope = this._serviceProvider.CreateScope())
                    {
                        try
                        {
                            var mediator = scope.ServiceProvider.GetService<IMediator>();

                            var decreasingItems = new List<DecreaseStock.OrderItem>();
                            objOrderPlacedEvent.Items.ForEach(item =>
                            {
                                decreasingItems.Add(new DecreaseStock.OrderItem
                                {
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity
                                });
                            });

                            await mediator.Send(new DecreaseStock.Command
                            {
                                MessageId = objOrderPlacedEvent.MessageId,
                                OrderId = objOrderPlacedEvent.OrderId,
                                Items = decreasingItems
                            });
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                }
            });
        }

        public class OrderPlaceEventProfile : Profile
        {
            public OrderPlaceEventProfile()
            {
                CreateMap<OrderPlacedEvent.OrderItem, DecreaseStock.OrderItem>();
            }
        }
    }
}

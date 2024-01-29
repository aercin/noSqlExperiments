using application.Features.Commands;
using core_application.Abstractions;
using core_messages;
using core_messages.IntegrationEvents;
using MediatR;
using System.Text.Json;

namespace api.Consumers
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IEventListener _eventListener;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        public OrderConsumer(IEventListener eventListener,
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

                if (integrationEventBase.EventType == typeof(StockDecreasedEvent).FullName)
                {
                    var objOrderSuccessedEvent = JsonSerializer.Deserialize<StockDecreasedEvent>(message);
                    using (var scope = this._serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetService<IMediator>();
                        await mediator.Send(new OrderSucceed.Command
                        {
                            OrderId = objOrderSuccessedEvent.OrderId,
                            MessageId = objOrderSuccessedEvent.MessageId
                        });
                    }
                }
                else if (integrationEventBase.EventType == typeof(StockNotDecreasedEvent).FullName)
                {
                    var objOrderFailedEvent = JsonSerializer.Deserialize<StockNotDecreasedEvent>(message);

                    using (var scope = this._serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetService<IMediator>();
                        await mediator.Send(new OrderFailed.Command
                        {
                            OrderId = objOrderFailedEvent.OrderId,
                            MessageId = objOrderFailedEvent.MessageId
                        });
                    }
                }
            });
        }
    }
}

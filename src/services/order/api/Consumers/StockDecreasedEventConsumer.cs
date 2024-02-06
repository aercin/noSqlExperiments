using application.Features.Commands;
using core_messages.IntegrationEvents;
using MassTransit;
using MediatR;

namespace api.Consumers
{
    public class StockDecreasedEventConsumer : IConsumer<StockDecreasedEvent>
    {
        private readonly IMediator _mediator;
        public StockDecreasedEventConsumer(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task Consume(ConsumeContext<StockDecreasedEvent> context)
        {
            await this._mediator.Send(new OrderSucceed.Command
            {
                OrderId = context.Message.OrderId,
                MessageId = context.ConversationId.Value
            });
        }
    }
}

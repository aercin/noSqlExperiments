using application.Features.Commands;
using core_messages;
using core_messages.IntegrationEvents;
using MassTransit;
using MediatR;

namespace api.Consumers
{
    public class StockNotDecreasedEventConsumer : IConsumer<StockNotDecreasedEvent>
    {
        private readonly IMediator _mediator;
        public StockNotDecreasedEventConsumer(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task Consume(ConsumeContext<StockNotDecreasedEvent> context)
        {
            await this._mediator.Send(new OrderFailed.Command
            {
                OrderId = context.Message.OrderId,
                MessageId = context.ConversationId.Value
            });
        }
    }
}

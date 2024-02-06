using application.Features.Commands;
using core_messages.IntegrationEvents;
using MassTransit; 
using MediatR;

namespace api.Consumers
{
    public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
    {
        private readonly IMediator _mediator;
        public OrderPlacedEventConsumer(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
        {
            var decreasingItems = new List<DecreaseStock.OrderItem>();
            context.Message.Items.ForEach(item =>
            {
                decreasingItems.Add(new DecreaseStock.OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            });

            await this._mediator.Send(new DecreaseStock.Command
            {
                MessageId = context.ConversationId.Value,
                OrderId = context.Message.OrderId,
                Items = decreasingItems
            });
        }
    }
}

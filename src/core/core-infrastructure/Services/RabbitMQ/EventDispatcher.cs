using core_application.Abstractions;
using MassTransit;

namespace core_infrastructure.Services.RabbitMQ
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IBus _bus;
        public EventDispatcher(IBus bus)
        {
            this._bus = bus;
        }

        public async Task DispatchEventAsync(object message)
        {
            await this._bus.Publish(message);
        }
    }
}

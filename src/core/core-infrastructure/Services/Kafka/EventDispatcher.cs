using Confluent.Kafka;
using core_application.Abstractions;

namespace core_infrastructure.Services.Kafka
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IProducer<Null, string> _producer;
        public EventDispatcher(IProducer<Null, string> producer)
        {
            this._producer = producer;
        }

        public async Task DispatchEventAsync(string topic, string message)
        {
            await this._producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = message
            });
        }
    }
}

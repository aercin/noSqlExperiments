using Confluent.Kafka;
using core_application.Abstractions;

namespace core_infrastructure.Services.Kafka
{
    public class EventListener : IEventListener
    {
        private IConsumer<Null, string> _consumer;
        public EventListener(IConsumer<Null, string> consumer)
        {
            this._consumer = consumer;
        }

        public async Task ConsumeEvent(string topic, Func<string, Task> callback)
        {
            this._consumer.Subscribe(topic);

            while (true)
            {
                try
                {
                    var response = this._consumer.Consume();

                    await callback(response.Message.Value);

                    this._consumer.Commit(response);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}

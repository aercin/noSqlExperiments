using Confluent.Kafka;
using core_application.Abstractions;
using core_infrastructure.Services.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace core_infrastructure.Extensions
{
    public static class KafkaDependency
    {
        public static void AddKafkaDependency(this IServiceCollection services, IConfiguration config)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IProducer<Null, string>>(x => new ProducerBuilder<Null, string>(new ProducerConfig
            {
                BootstrapServers = config.GetValue<string>("Integration:Async:BrokerAddress"),
                Acks = Acks.Leader
            }).Build());

            services.AddSingleton<IEventDispatcher, EventDispatcher>();

            services.AddSingleton<IConsumer<Null, string>>(x => new ConsumerBuilder<Null, string>(new ConsumerConfig
            {
                BootstrapServers = config.GetValue<string>("Integration:Async:BrokerAddress"),
                GroupId = config.GetValue<string>("Integration:Async:ConsumerGroupId"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            }).Build());

            services.AddSingleton<IEventListener, EventListener>();
        }
    }
}

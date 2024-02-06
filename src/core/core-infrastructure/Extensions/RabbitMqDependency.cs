using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace core_infrastructure.Extensions
{
    public static class RabbitMqDependency
    {
        public static void AddRabbitMqDependency(this IServiceCollection services, IConfiguration option)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var consumeOptions = option.GetSection("Integration:Async:Consume").Get<List<ConsumeOption>>();

            services.AddMassTransit(config =>
            {
                if (consumeOptions != null)
                {
                    consumeOptions.SelectMany(x => x.Consumers).ToList().ForEach(consumer =>
                    {
                        config.AddConsumer(Type.GetType(consumer.Type));
                    });
                } 

                config.UsingRabbitMq((cxt, cfg) =>
                {
                    cfg.Host(new Uri(option.GetValue<string>("Integration:Async:BrokerAddress")), x =>
                    {
                        x.Username(option.GetValue<string>("RabbitMQ:Username"));
                        x.Password(option.GetValue<string>("RabbitMQ:Password"));
                    });

                    if (consumeOptions != null)
                    {
                        consumeOptions.ForEach(consumeOption =>
                        {
                            cfg.ReceiveEndpoint(consumeOption.Queue, ep =>
                            {
                                consumeOption.Consumers.ForEach(consumer =>
                                {
                                    ep.UseMessageRetry(x => x.Interval(consumer.MaxRetry, TimeSpan.FromMilliseconds(consumer.RetryInterval)));
                                    ep.ConfigureConsumer(cxt, Type.GetType(consumer.Type));
                                });
                            });
                        });
                    }
                });
            });
        }

        private sealed class ConsumeOption
        {
            public string Queue { get; set; }

            public List<ConsumerOption> Consumers { get; set; }
        }

        private sealed class ConsumerOption
        {
            public string Type { get; set; }
            public int MaxRetry { get; set; }
            public int RetryInterval { get; set; }
        }

    }
}

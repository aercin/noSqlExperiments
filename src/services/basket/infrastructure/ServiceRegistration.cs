using application;
using application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddApplication();

            var redisConfigOption = ConfigurationOptions.Parse(config.GetValue<string>("Redis:Endpoints"));
            redisConfigOption.Password = config.GetValue<string>("Redis:Password");
            redisConfigOption.DefaultDatabase = config.GetValue<int>("Redis:Database");

            services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(redisConfigOption));

            services.AddScoped<IRedisRepository, RedisRepository>();
        } 
    }
}

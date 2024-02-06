using application;
using application.Abstractions;
using core_application.Abstractions;
using core_infrastructure.Extensions;
using core_infrastructure.Services.RabbitMQ;
using infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddApplication();
             
            services.AddScoped<IOrderRepository, OrderRepository>(); 
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMongoDependency(x =>
            {
                x.ConnectionString = $"mongodb://{config.GetValue<string>("MongoDb:Host")}:{config.GetValue<string>("MongoDb:Port")}/?replicaSet=rs0";
            });
            services.AddRabbitMqDependency(config);
            services.AddCorrelation(); 
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
        }
    }
}

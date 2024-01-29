using application;
using application.Abstractions;
using core_infrastructure.Extensions;
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

            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMongoDependency(x =>
            {
                x.ConnectionString = $"mongodb://{config.GetValue<string>("MongoDb:Host")}:{config.GetValue<string>("MongoDb:Port")}/?replicaSet=rs0";
            });
            services.AddKafkaDependency(config);
            services.AddCorrelation();
        }
    }
}

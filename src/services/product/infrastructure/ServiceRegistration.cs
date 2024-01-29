using application;
using application.Abstractions;
using core_infrastructure.Extensions;
using Couchbase.Extensions.DependencyInjection;
using infrastructure.Services;
using Microsoft.AspNetCore.Builder;
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

            services.AddCouchbase(x =>
            {
                x.ConnectionString = config.GetValue<string>("Couchbase:ConnectionString");
                x.UserName = config.GetValue<string>("Couchbase:UserName");
                x.Password = config.GetValue<string>("Couchbase:Password");
                x.HttpIgnoreRemoteCertificateMismatch = true;
                x.KvIgnoreRemoteCertificateNameMismatch = true;
            });

            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddCorrelation();
        }

        public static void AddInfrastructuralPipelines(this WebApplication app)
        {
            app.Lifetime.ApplicationStopped.Register(() =>
            {
                app.Services.GetService<ICouchbaseLifetimeService>()?.Close();
            });
        }
    }
}
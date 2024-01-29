using core_application.Abstractions;
using core_infrastructure.Services.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace core_infrastructure.Extensions
{
    public static class MongoDependency
    {
        public static void AddMongoDependency(this IServiceCollection services, Action<MongoOptions> options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var mongoOptions = new MongoOptions();
            options(mongoOptions);

            services.AddSingleton<IMongoClient>(s => new MongoClient(mongoOptions.ConnectionString));
            services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
            services.AddScoped<IInboxMessageRepository, InboxMessageRepository>();
        }

        public class MongoOptions
        {
            public string ConnectionString { get; set; }
        }
    }
}

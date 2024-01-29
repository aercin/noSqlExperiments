using core_application.Abstractions;
using core_infrastructure.Services.Correlation;
using Microsoft.Extensions.DependencyInjection;

namespace core_infrastructure.Extensions
{
    public static class CorrelationDependency
    {
        public static void AddCorrelation(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddHttpContextAccessor();
            services.AddSingleton<ICorrelationService, CorrelationService>();
        }
    }
}

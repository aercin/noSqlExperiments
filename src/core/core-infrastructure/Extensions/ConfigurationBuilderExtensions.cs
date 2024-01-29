using core_infrastructure.Services.Vault;
using Microsoft.Extensions.Configuration;

namespace core_infrastructure.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddVaultConfiguration(this IConfigurationBuilder builder, Action<VaultConfigurationOptions> options)
        {
            var settings = new VaultConfigurationOptions();

            options(settings);

            return builder.Add(new VaultConfigurationSource(settings));
        }
    }
}

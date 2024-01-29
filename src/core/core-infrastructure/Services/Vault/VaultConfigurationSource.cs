using Microsoft.Extensions.Configuration;

namespace core_infrastructure.Services.Vault
{
    public class VaultConfigurationSource : IConfigurationSource
    {
        private VaultConfigurationOptions _options;
        public VaultConfigurationSource(VaultConfigurationOptions options)
        {
            this._options = options;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new VaultConfigurationProvider(this._options);
        }
    }
}

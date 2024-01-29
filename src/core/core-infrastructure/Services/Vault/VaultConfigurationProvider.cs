using Microsoft.Extensions.Configuration;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.Commons;

namespace core_infrastructure.Services.Vault
{
    public class VaultConfigurationProvider : ConfigurationProvider
    {
        private readonly VaultConfigurationOptions _options;
        private Task _watchVaultTask;
        private readonly IVaultClient _vaultClient;

        public VaultConfigurationProvider(VaultConfigurationOptions options)
        {
            this._options = options;

            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(options.AuthToken);

            var vaultClientSettings = new VaultClientSettings(options.Address, authMethod);

            this._vaultClient = new VaultClient(vaultClientSettings);
        }

        public override void Load()
        {
            if (_watchVaultTask != null)
            {
                return;
            }

            GetDataAsync().Wait();

            if (this._options.ReloadOnChange)
            {
                _watchVaultTask = WatchVaultAsync();
            }
        }

        private async Task WatchVaultAsync()
        {
            while (true)
            {
                await Task.Delay(this._options.PollingInterval);

                await GetDataAsync();
            }
        }

        private async Task GetDataAsync()
        {
            try
            {
                Secret<SecretData> kv2Secret = await this._vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: this._options.Path, mountPoint: this._options.MountPoint).ConfigureAwait(false);

                if (kv2Secret?.Data != null)
                {
                    Data = kv2Secret.Data.Data.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
                }

            }
            catch (Exception ex)
            {
                if (this._options.OnLoadException != null)
                {
                    this._options.OnLoadException(ex);
                }
            }
        }
    }
}

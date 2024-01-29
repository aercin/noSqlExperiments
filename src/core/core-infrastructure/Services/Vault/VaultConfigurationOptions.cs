namespace core_infrastructure.Services.Vault
{
    public class VaultConfigurationOptions
    {
        public string MountPoint { get; set; }
        public string Path { get; set; }
        public string AuthToken { get; set; }
        public string Address { get; set; }
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// Miliseconds
        /// </summary>
        public int PollingInterval { get; set; } = 60000;

        public Action<Exception> OnLoadException { get; set; }
    }
}

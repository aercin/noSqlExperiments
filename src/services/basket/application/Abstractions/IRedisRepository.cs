namespace application.Abstractions
{
    public interface IRedisRepository
    {
        public Task<T> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T item, TimeSpan expiration);
        public Task RemoveAsync(string key);
        public Task<bool> IsKeyExistAsync(string key);
    }
}

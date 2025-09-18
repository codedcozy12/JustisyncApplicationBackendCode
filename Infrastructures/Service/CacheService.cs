using Application.Interfaces.ExternalService;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructures.Service
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task AddAsync<T>(string key, T Value, TimeSpan? span)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = span ?? TimeSpan.FromMinutes(10)
            };
            _memoryCache.Set(key,Value,options);
            return Task.CompletedTask;
        }
        public Task DeleteAsync<T>(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        public Task<T?> GetAsync<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T? value))
                return Task.FromResult(value);
            return Task.FromResult(default(T));
        }
    }
}
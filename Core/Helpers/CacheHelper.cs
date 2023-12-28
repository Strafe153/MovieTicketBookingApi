using Core.Interfaces.Helpers;
using Core.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Helpers
{
    public class CacheHelper : ICacheHelper
    {
        private readonly IMemoryCache _memoryCache;

        public CacheHelper(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key) => _memoryCache.Get<T>(key);

        public void Set<T>(string key, T value, CacheOptions options) =>
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            });
    }
}

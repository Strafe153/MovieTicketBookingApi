using Core.Interfaces.Helpers;
using Core.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Core.Helpers;

public class CacheHelper : ICacheHelper
{
	private readonly IMemoryCache _memoryCache;
	private readonly MemoryCacheEntryOptions _cacheOptions;

	public CacheHelper(
		IMemoryCache memoryCache,
		IOptions<MemoryCacheEntryOptions> cacheOptions)
	{
		_memoryCache = memoryCache;
		_cacheOptions = cacheOptions.Value;
	}

	public T? Get<T>(string key) => _memoryCache.Get<T>(key);

	public void Set<T>(string key, T value) => _memoryCache.Set(key, value, _cacheOptions);
}
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using redis_example_net.Interface;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T> Get<T>(string key)
    {
        var value = await _cache.GetStringAsync(key);

        if (value != null)
        {
            return JsonConvert.DeserializeObject<T>(value)!;
        }

        return default!;
    }

    public async Task Set<T>(string key, T value, TimeSpan? expires)
    {
        var timeOut = new DistributedCacheEntryOptions
        {
            // 현재 시간 기준 만료 시간 설정
            AbsoluteExpirationRelativeToNow = expires == null ? TimeSpan.FromSeconds(20) : expires,
            // 일정 시간 동안 Access 하지 않을떄 만료 시간 설정
            SlidingExpiration = TimeSpan.FromSeconds(10)
        };

        await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), timeOut);
    }
}
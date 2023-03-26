using redis_example_net.Interface;

using StackExchange.Redis;

public class RedisService : IRedisService
{
    private readonly IDatabase _redis;

    public RedisService(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task<RedisValue> GetString(string key)
    {
        return await _redis.StringGetAsync(key, CommandFlags.PreferReplica);
    }

    public async Task SetString(string key, object value) 
    {
        await _redis.StringSetAsync(key, (RedisValue)value);
    }

    public async Task SetStream(string key, Dictionary<String, String> dic) {
        await _redis.StreamAddAsync(
            key,
            dic.Select(s => new NameValueEntry(s.Key, (RedisValue)s.Value)).ToArray()
        );
    }
}
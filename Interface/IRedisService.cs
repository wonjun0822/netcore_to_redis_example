using StackExchange.Redis;

namespace redis_example_net.Interface;

public interface IRedisService
  {
      public Task<RedisValue> GetString(string key);
      public Task SetString(string key, object value);

      public Task SetStream(string key, Dictionary<String, String> dic);
  }
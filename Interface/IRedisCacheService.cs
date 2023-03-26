namespace redis_example_net.Interface;

public interface IRedisCacheService
  {
      public Task<T> Get<T>(string key);
      public Task Set<T>(string key, T value, TimeSpan? expires);
  }
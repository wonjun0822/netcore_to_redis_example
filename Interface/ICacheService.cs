namespace redis_example_net.Interface;

public interface ICacheService {
    public Task<Dictionary<string, string>> GetCache(string id);
}
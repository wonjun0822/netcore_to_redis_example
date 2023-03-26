using redis_example_net.Interface;

namespace redis_example_net.Service;

public class CacheService : ICacheService
{
    private readonly IRedisCacheService _redisCacheService;

    public CacheService(IRedisCacheService redisCacheService) {
        _redisCacheService = redisCacheService;
    }

    public async Task<Dictionary<string, string>> GetCache(string id)
    {
        string key = $"cache:{id}";

        Dictionary<string, string> cache = await _redisCacheService.Get<Dictionary<String, String>>(key);

        if (cache == null) {
            Dictionary<String, String> tmp = new Dictionary<string, string>();

            try {
                Thread.Sleep(500);
            }

            catch (ThreadInterruptedException) {
            }

            tmp.Add("id", id);
            tmp.Add("title", "제목");
            tmp.Add("content", "내용");

            await _redisCacheService.Set<Dictionary<string, string>>(key, tmp, null);

            return tmp;
        }

        else return cache;
    }
}
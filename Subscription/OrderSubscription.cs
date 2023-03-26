using StackExchange.Redis;

namespace redis_example_net.Subscription;

public class OrderSubscription : BackgroundService
{
    private readonly IDatabase _database;
    private readonly string stream = "order";
    private readonly string group = "group1";

    public OrderSubscription(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // stream consumer group check
        if (await ConsumerExistsAsync())
        {
            // consumer group이 없다면 group 생성
            await ConsumerCreateAsync();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            // stream message read
            var result = await _database.StreamReadGroupAsync(stream, group, "order", ">", 1);

            if (result.Any())
            {
                var dict = ParseResult(result.First());

                Console.WriteLine(string.Join(Environment.NewLine, dict.Select(s => $"{s.Key}, {s.Value}")));

                // payment subscribe 테스트를 위한 stream 생성
                await AddToStreamAsync("payment", dict);
            }
        }
    }

    private async Task<bool> ConsumerExistsAsync() => !(await _database.KeyExistsAsync(stream)) || (await _database.StreamGroupInfoAsync(stream)).All(x => x.Name != group);

    private async Task ConsumerCreateAsync() => await _database.StreamCreateConsumerGroupAsync(stream, group, StreamPosition.NewMessages, true);

    private async Task AddToStreamAsync(string streamName, Dictionary<string, string> values)
    {
        var entries = values.Select(s => new NameValueEntry(s.Key, (RedisValue)s.Value)).ToArray();
        
        await _database.StreamAddAsync(streamName, entries);
    }

    private Dictionary<string, string> ParseResult(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());

    public override Task StopAsync(CancellationToken stoppingToken) => base.StopAsync(stoppingToken);
}
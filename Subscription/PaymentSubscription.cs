using StackExchange.Redis;

namespace redis_example_net.Subscription;

public class PaymentSubscription : BackgroundService
{
    private readonly IDatabase _database;
    private readonly string stream = "payment";
    private readonly string group = "group1";

    public PaymentSubscription(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await StreamExistsAsync())
        {
            await CreateStreamAsync();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _database.StreamReadGroupAsync(stream, group, "payment", ">", 1);

            if (result.Any())
            {
                var dict = ParseResult(result.First());
                
                Console.WriteLine(string.Join(Environment.NewLine, dict.Select(s => $"{s.Key}, {s.Value}")));
            }
        }
    }

    private async Task<bool> StreamExistsAsync()
    {
        return await _database.KeyExistsAsync(stream);
    }

    private async Task CreateStreamAsync()
    {
        await _database.StreamCreateConsumerGroupAsync(stream, group, StreamPosition.NewMessages, true);
    }

    private Dictionary<string, string> ParseResult(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
}

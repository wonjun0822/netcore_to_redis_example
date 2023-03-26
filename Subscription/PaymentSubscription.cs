using StackExchange.Redis;

namespace redis_example_net.Subscription;

public class PaymentSubscription : BackgroundService
{
    private readonly IDatabase _redis;

    public PaymentSubscription(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const string streamName = "payment";
        const string groupName = "group1";

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;

        string id = string.Empty;

        if (!(await _redis.KeyExistsAsync(streamName)) || (await _redis.StreamGroupInfoAsync(streamName)).All(x => x.Name != groupName))
        {
            await _redis.StreamCreateConsumerGroupAsync(streamName, groupName, "0-0", true);
        }

        while (!token.IsCancellationRequested)
        {    
            if (!string.IsNullOrEmpty(id))
            {
                await _redis.StreamAcknowledgeAsync(streamName, groupName, id);

                id = string.Empty;
            }

            var result = await _redis.StreamReadGroupAsync(streamName, groupName, "payment", ">", 1);

            if (result.Any()) 
            {
                var dict = ParseResult(result.First());
                
                Console.WriteLine(string.Join(Environment.NewLine, dict.Select(s => $"{s.Key}, {s.Value}")));
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        await base.StopAsync(stoppingToken);
    }

    Dictionary<string, string> ParseResult(StreamEntry entry) => entry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
}
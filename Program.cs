using Microsoft.AspNetCore.DataProtection;
using redis_example_net.Interface;
using redis_example_net.Service;
using redis_example_net.Subscription;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var redisConnection = builder.Configuration.GetConnectionString("Redis")?.ToString();
var redisKey = builder.Configuration.GetSection("Redis:Key").Value;

var redis = ConnectionMultiplexer.Connect(redisConnection!);

// Redis 데이터 접근 보호 Key 설정
builder.Services
    .AddDataProtection()
    .PersistKeysToStackExchangeRedis(redis, redisKey);

// Redis Cache 설정
builder.Services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = redisConnection;
});

// Session 설정
builder.Services.AddSession(o => {
    o.Cookie.Name = "session";
    o.Cookie.HttpOnly = true;
    o.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IRedisService, RedisService>();

builder.Services.AddHostedService<OrderSubscription>();
builder.Services.AddHostedService<PaymentSubscription>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthorization();

app.MapControllers();

app.Run();

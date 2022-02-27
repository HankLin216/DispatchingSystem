using StackExchange.Redis;
using System.Text.Json;

namespace DispatchingSystem.DB;

public class RedisService
{
    private readonly string _redisHost;
    private readonly int _redisPort;
    private ConnectionMultiplexer _redis;
    private IDatabase db0;
    public RedisService(IConfiguration config)
    {
        _redisHost = config["Redis:Host"];
        _redisPort = Convert.ToInt32(config["Redis:Port"]);

        // connect to redis
        var configString = $"{_redisHost}:{_redisPort},connectRetry=5";
        _redis = ConnectionMultiplexer.Connect(configString);

        // create default db
        db0 = _redis.GetDatabase();
    }

    public async Task<bool> StringSetAsync(string key, string value)
    {
        return await db0.StringSetAsync(key, value);
    }

    public async Task<string> StringGetAsync(string key)
    {
        return await db0.StringGetAsync(key);
    }

    public async Task HashSetAsync(string key, HashEntry[] kvPairs)
    {
        await db0.HashSetAsync(key, kvPairs);
    }

}


using Courses.Core.RedisRepository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Courses.Repo.RedisRepository
{
    public class RedisRepo<T> : IRedisRepo<T>
    {
        protected readonly IDatabase _redisDb;
        protected readonly string _keyPrefix;
        protected readonly ILogger<RedisRepo<T>> _logger;

        public RedisRepo(IConnectionMultiplexer redis, string keyPrefix, ILogger<RedisRepo<T>> logger)
        {
            _redisDb = redis.GetDatabase();
            _keyPrefix = keyPrefix;
            _logger = logger;
        }

        private string GetFullKey(string key) => $"{_keyPrefix}:{key}";

        public async Task<bool> SetKeyAsync(string Key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var fullKey = GetFullKey(Key);
                var json = JsonConvert.SerializeObject(value);

                return await _redisDb.StringSetAsync(fullKey, json, expiry.HasValue ? (Expiration)expiry : default);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<T?> GetKeyAsync(string key)
        {
            try
            {
                var fullKey = GetFullKey(key);
                var value = await _redisDb.StringGetAsync(fullKey);
                if (value.HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(value!);
                }
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default;
            }
        }

        public async Task<bool> DeleteKeyAsync(string key)
        {
            try
            {
                var fullKey = GetFullKey(key);
                return await _redisDb.KeyDeleteAsync(fullKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<bool> KeyExistAsync(string key)
        {
            var fullKey = GetFullKey(key);
            return await _redisDb.KeyExistsAsync(fullKey);
        }
    }
}

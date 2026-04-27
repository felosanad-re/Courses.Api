namespace Courses.Core.RedisRepository
{
    public interface IRedisRepo<T>
    {
        // Set Key
        Task<bool> SetKeyAsync(string Key, T value, TimeSpan? expiry = null);
        // Get Key
        Task<T?> GetKeyAsync(string key);
        // Deleted Key
        Task<bool> DeleteKeyAsync(string key);
        // Key Exist
        Task<bool> KeyExistAsync(string key);
    }
}

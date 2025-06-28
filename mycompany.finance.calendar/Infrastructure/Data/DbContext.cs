namespace Mycompany.Finance.Calendar.Infrastructure.Data;

using Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

/// <summary>
/// Represents a database context for data access.
/// </summary>
public class DbContext : IDbContext
{
    /// <summary>
    /// The Redis database used for data access.
    /// </summary>
    private readonly IDatabase _redisDb;

    /// <summary>
    /// The default time-to-live (TTL) for cached items in the database.
    /// </summary>
    private readonly TimeSpan _defaultTtl;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContext"/> class.
    /// </summary>
    /// <param name="redis"> The Redis connection multiplexer used to access the database.</param>
    /// <param name="configuration"></param>
    public DbContext(IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _redisDb = redis.GetDatabase();

        // Read TTL from config (in minutes)
        var ttlMinutes = configuration.GetValue<int>("Redis:DefaultTtlMinutes", 5000);
        _defaultTtl = TimeSpan.FromMinutes(ttlMinutes);
    }

    /// <summary>
    /// Retrieves a value from the database using the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to access the value.</param>
    /// <returns>The value associated with the key, or null if not found.</returns>
    public async Task<T?> GetData<T>(IDataKey key) where T : class
    {
        var value = await _redisDb.StringGetAsync(key.Identifier);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : null;
    }

    /// <summary>
    /// Retrieves a value from the database using the specified key and type.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to access the value.</param>
    /// <param name="field"></param>
    /// <returns>The value associated with the key, or null if not found.</returns>
    public async Task<T?> GetHashData<T>(IDataKey key, string field) where T : class
    {
        if (string.IsNullOrEmpty(field))
            throw new ArgumentException("Hash field cannot be null or empty.", nameof(field));

        // Retrieve the value for the specific hash field
        var value = await _redisDb.HashGetAsync(key.Identifier, field);

        // Configure deserialization options
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // If a value exists, deserialize and return; otherwise, return null
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!, options) : null;
    }
}
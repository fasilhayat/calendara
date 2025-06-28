namespace Mycompany.Finance.Calendar.Infrastructure.Data;

using Core.Interfaces;

/// <summary>
/// Represents a database context for data access.
/// </summary>
public interface IDbContext
{
    /// <summary>
    /// Retrieves a value from the database using the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to access the value.</param>
    /// <returns>The value associated with the key, or null if not found.</returns>
    Task<T?> GetData<T>(IDataKey key) where T : class;

    /// <summary>
    /// Retrieves a value from the database using the specified key and type.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to access the value.</param>
    /// <param name="field">The field indicator in HashData.</param>
    /// <returns>The value associated with the key, or null if not found.</returns>
    Task<T?> GetHashData<T>(IDataKey key, string field) where T : class;
}
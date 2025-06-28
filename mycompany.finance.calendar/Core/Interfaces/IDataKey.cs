namespace Mycompany.Finance.Calendar.Core.Interfaces;

/// <summary>
/// Represents a key for data access.
/// </summary>
public interface IDataKey
{
    /// <summary>
    /// Initializes a new instance of data class with the specified identifier.
    /// </summary>
    string Identifier { get; init; }
}
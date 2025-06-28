namespace Mycompany.Finance.Calendar.Core.Entities;

/// <summary>
/// Represents a holiday with a specific date and an optional description.
/// </summary>
public class Holiday
{
    // The date of the holiday
    public DateTime Date { get; init; }

    // The local name of the holiday
    public string? LocalName { get; init; }

    // The common name of the holiday
    public string? Name { get; init; }

    // The country code where the holiday applies
    public string? CountryCode { get; init; }

    // Whether the holiday occurs on a fixed date every year
    public bool Fixed { get; init; }

    // Whether the holiday is globally observed
    public bool Global { get; init; }

    // The specific counties where the holiday applies (can be null if it applies nationwide)
    public List<string>? Counties { get; init; }

    // The year the holiday was first launched (can be null if unknown)
    public int? LaunchYear { get; init; }

    // The types of the holiday (e.g., Public, Bank, School, etc.)
    public List<string>? Types { get; init; }
}
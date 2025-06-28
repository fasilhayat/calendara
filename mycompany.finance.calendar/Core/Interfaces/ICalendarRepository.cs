namespace Mycompany.Finance.Calendar.Core.Interfaces;

using Entities;

/// <summary>
/// Interface for accessing holiday related data.
/// </summary>
public interface ICalendarRepository
{
    /// <summary>
    /// Retrieves a holiday description for the specified date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="date">The date for which the holiday description is to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing the holiday description.</returns>
    Task<Holiday?> GetHolidayAsync(string countryCode, DateTime date);


    /// <summary>
    /// Retrieves holidays  for the specified year and country.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="year">The year from where the holidays needs to be retrieved</param>
    /// <returns>List of holidays for year and country</returns>
    Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, int year);

    /// <summary>
    /// Retrieves holiday descriptions for a range of dates.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="fromDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns> A task representing the asynchronous operation, containing a collection of holiday descriptions within the specified date range.</returns>

    Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, DateTime fromDate, DateTime endDate);
}
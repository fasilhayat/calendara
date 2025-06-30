namespace Mycompany.Finance.Calendar.Application.Services;

using Core.Entities;
using Core.Interfaces;

/// <summary>
/// The CalendarService class provides methods to manage calendar related data.
/// </summary>
public class CalendarService
{
    /// <summary>
    /// A reference to the calendar repository used for accessing calendar-related data, such as holidays.
    /// </summary>
    private readonly ICalendarRepository _calendarRepository;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="calendarRepository">The calendar repository.</param>
    public CalendarService(ICalendarRepository calendarRepository)
    {
        _calendarRepository = calendarRepository;
    }

    /// <summary>
    /// Retrieves a holiday or weekend description for the specified date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="date">The date for which the holiday description is to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing the holiday description.</returns>
    public async Task<Holiday?> GetHolidayOrWeekendAsync(string countryCode, DateTime date)
    {
        return await _calendarRepository.GetHolidayOrWeekendAsync(countryCode, date);
    }

    /// <summary>
    /// Retrieves a holiday description for the specified date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="year">The year for which the holiday description is to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing the holiday description.</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, int year)
    {
        return await _calendarRepository.GetHolidaysAsync(countryCode, year);
    }

    /// <summary>
    /// Retrieves holiday descriptions for a range of dates.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="fromDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns> A task representing the asynchronous operation, containing a collection of holiday descriptions 
    /// within the specified date range.</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, DateTime fromDate, DateTime endDate)
    {
        return await _calendarRepository.GetHolidaysAsync(countryCode, fromDate, endDate);
    }
}
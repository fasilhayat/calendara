namespace Mycompany.Finance.Calendar.Infrastructure.Repositories;

using Core.Entities;
using Core.Interfaces;
using Data;

/// <summary>
/// Repository for accessing calendar-related data, such as holidays.
/// Implements the ICalendarRepository interface.
/// </summary>
public class CalendarRepository : ICalendarRepository
{
    /// <summary>
    /// The database context used for data access.
    /// </summary>
    private readonly IDbContext _context;

    /// <summary>
    /// The constructor for the EmployeeRepository class.
    /// </summary>
    /// <param name="context">The context object.</param>
    public CalendarRepository(IDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a holiday description for the specified date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="date">The date for which the holiday description is to be retrieved.</param>
    /// <returns>A task representing the asynchronous operation, containing the holiday description.</returns>
    public async Task<Holiday?> GetHolidayAsync(string countryCode, DateTime date)
    {
        // Form the Redis hash key using the year
        var key = new DataKey($"holiday:{date.Year}");

        // Retrieve the holidays for the specified country code
        var holidays = await _context.GetHashData<IEnumerable<Holiday>>(key, countryCode);
        var holiday = holidays?.FirstOrDefault(h => h.Date.Date == date.Date);

        return holiday;
    }

    /// <summary>
    /// Retrieves holidays  for the specified year and country.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="year">The year from where the holidays needs to be retrieved</param>
    /// <returns>List of holidays for year and country</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, int year)
    {
        // Form the Redis hash key using the year
        var key = new DataKey($"holiday:{year}");

        // Retrieve the holidays for the specified country code
        var holidays = await _context.GetHashData<IEnumerable<Holiday>>(key, countryCode);

        return holidays ?? [];
    }

    /// <summary>
    /// Retrieves holiday descriptions for a range of dates.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="fromDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns> A task representing the asynchronous operation, containing a collection of holiday descriptions within the specified date range.</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, DateTime fromDate, DateTime endDate)
    {
        var years = GetYearsBetween(fromDate, endDate);

        // Create a list of data keys for each year in the range
        var dataKeys = years.Select(year => new DataKey($"holiday:{year}")).ToList();

        var allHolidays = new List<Holiday>();

        // Retrieve and accumulate holidays for the specified country code across all years
        foreach (var dataKey in dataKeys)
        {
            var holidaysForYear = await _context.GetHashData<IEnumerable<Holiday>>(dataKey, countryCode);
            if (holidaysForYear != null)
            {
                allHolidays.AddRange(holidaysForYear);
            }
        }

        // Filter by date range and return
        return allHolidays
            .Where(h => h.Date >= fromDate && h.Date <= endDate)
            .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private IEnumerable<int> GetYearsBetween(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
            throw new ArgumentException("fromDate must be earlier than or equal to toDate");

        var startYear = fromDate.Year;
        var endYear = toDate.Year;

        var years = new int[endYear - startYear + 1];
        for (var i = 0; i <= endYear - startYear; i++)
        {
            years[i] = startYear + i;
        }

        return years;
    }
}
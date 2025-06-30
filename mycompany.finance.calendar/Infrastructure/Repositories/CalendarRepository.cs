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
    private readonly IDbContext _context;

    /// <summary>
    /// Constructor for CalendarRepository.
    /// </summary>
    /// <param name="context">The database context used for data access.</param>
    public CalendarRepository(IDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a holiday or weekend for a specified date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format, or "EU" for EU-specific holidays.</param>
    /// <param name="date">The date for which the holiday or weekend is being checked.</param>
    /// <returns>A <see cref="Holiday"/> object if the date matches a holiday or falls on a weekend; otherwise, null.</returns>
    public async Task<Holiday?> GetHolidayOrWeekendAsync(string countryCode, DateTime date)
    {
        if (countryCode.Equals("EU", StringComparison.OrdinalIgnoreCase))
            return await GetHolidayForEuOrWeekendAsync(date);

        // Handle holidays for other countries
        return await GetHolidayForCountryOrWeekendAsync(countryCode, date);
    }

    /// <summary>
    /// Retrieves holidays for the specified year and country.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format, or "EU" for EU-specific holidays.</param>
    /// <param name="year">The year for which the holidays are retrieved.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a list of holidays.</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, int year)
    {
        if (countryCode.Equals("EU", StringComparison.OrdinalIgnoreCase))
            return await GetEuHolidaysAsync(year);

        var key = new DataKey($"holiday:{year}");
        var holidays = await _context.GetHashData<IEnumerable<Holiday>>(key, countryCode);
        return holidays ?? [];
    }

    /// <summary>
    /// Retrieves holiday descriptions for a range of dates.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format, or "EU" for EU-specific holidays.</param>
    /// <param name="fromDate">The start date of the date range.</param>
    /// <param name="endDate">The end date of the date range.</param>
    /// <returns>A collection of holiday descriptions within the specified date range.</returns>
    public async Task<IEnumerable<Holiday>> GetHolidaysAsync(string countryCode, DateTime fromDate, DateTime endDate)
    {
        var years = GetYearsBetween(fromDate, endDate);

        var holidayTasks = years.Select(year => GetHolidaysAsync(countryCode, year));
        var holidayResults = await Task.WhenAll(holidayTasks);

        return holidayResults
            .SelectMany(x => x)
            .Where(x => x.Date >= fromDate && x.Date <= endDate);
    }

    /// <summary>
    /// Retrieves an EU holiday or weekend for a specified date.
    /// </summary>
    /// <param name="date">The date for which the holiday or weekend is being checked.</param>
    /// <returns>A <see cref="Holiday"/> object if the date matches an EU holiday or falls on a weekend; otherwise, null.</returns>
    private async Task<Holiday?> GetHolidayForEuOrWeekendAsync(DateTime date)
    {
        var holidays = await GetEuHolidaysAsync(date.Year);
        var holiday = holidays?.FirstOrDefault(h => h.Date.Date == date.Date);
        return holiday ?? DetectWeekend(date, "EU");
    }

    /// <summary>
    /// Retrieves a holiday or weekend for a specified country and date.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format.</param>
    /// <param name="date">The date for which the holiday or weekend is being checked.</param>
    /// <returns>A <see cref="Holiday"/> object if the date matches a holiday or falls on a weekend; otherwise, null.</returns>
    private async Task<Holiday?> GetHolidayForCountryOrWeekendAsync(string countryCode, DateTime date)
    {
        var key = new DataKey($"holiday:{date.Year}");
        var holidaysForCountry = await _context.GetHashData<IEnumerable<Holiday>>(key, countryCode);
        var holiday = holidaysForCountry?.FirstOrDefault(h => h.Date.Date == date.Date);
        return holiday ?? DetectWeekend(date, countryCode);
    }

    /// <summary>
    /// Retrieves the list of EU holidays for a specific year.
    /// </summary>
    /// <param name="year">The year for which the EU holidays are retrieved.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation, with a list of EU holidays.</returns>
    private async Task<List<Holiday>> GetEuHolidaysAsync(int year)
    {
        // Simulate async behavior (e.g., database or API call)
        await Task.CompletedTask;
        return 
            [
            new()
                {
                    Name = "New Year's Day",
                    LocalName = "New Year's Day",
                    Date = new DateTime(year, 1, 1),
                    CountryCode = "EU",
                    Fixed = true,
                    Global = true,
                    Types = ["Public", "Bank", "School"]
                },
            new()
                {
                    Name = "Christmas Day",
                    LocalName = "Christmas Day",
                    Date = new DateTime(year, 12, 25),
                    CountryCode = "EU",
                    Fixed = true,
                    Global = true,
                    Types = ["Public", "Bank", "School"]
                }
            ];
    }

    /// <summary>
    /// Checks if the given date falls on a weekend and returns a synthetic holiday object if it does.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="countryCode">The country code to associate with the weekend holiday.</param>
    /// <returns>A <see cref="Holiday"/> object for weekends, or null if not a weekend.</returns>
    private Holiday? DetectWeekend(DateTime date, string countryCode)
    {
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return new Holiday
            {
                Name = date.DayOfWeek == DayOfWeek.Saturday ? "Weekend (Saturday)" : "Weekend (Sunday)",
                LocalName = date.DayOfWeek == DayOfWeek.Saturday ? "Weekend (Saturday)" : "Weekend (Sunday)",
                Date = date,
                CountryCode = countryCode,
                Fixed = false,
                Global = false,
                Types = ["Public", "Bank", "School"]
            };
        }

        return null;
    }



    /// <summary>
    /// Retrieves a sequence of years between two dates, inclusive of the years in both the start and end dates.
    /// </summary>
    /// <param name="fromDate">The starting date of the range.</param>
    /// <param name="toDate">The ending date of the range.</param>
    /// <returns>A sequence of years between the two dates, inclusive.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="fromDate"/> is later than <paramref name="toDate"/>.</exception>

    private IEnumerable<int> GetYearsBetween(DateTime fromDate, DateTime toDate)
    {
        if (fromDate > toDate)
            throw new ArgumentException("fromDate must be earlier than or equal to toDate");

        return Enumerable.Range(fromDate.Year, toDate.Year - fromDate.Year + 1);
    }
}
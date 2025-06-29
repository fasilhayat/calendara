namespace Mycompany.Finance.Calendar.Endpoints;

using Application.Services;

/// <summary>
/// Calendar endpoint.
/// </summary>
public static class CalendarEndpoint
{
    /// <summary>
    /// Calendar Endpoint.
    /// </summary>
    public static void MapCalendarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var calendar = endpoints.MapGroup("/v1/calendar").WithTags("Calendar");

        calendar.MapGet("/holiday/{countryCode}/{date}",
            static (string countryCode, DateTime date, CalendarService calendarService) => 
                GetHoliday(countryCode, date, calendarService));

        calendar.MapGet("/holidays/{countryCode}/{year}",
            static (string countryCode, int year, CalendarService calendarService) => 
                GetHolidays(countryCode, year, calendarService));

        // Endpoint to get holidays within a date range
        calendar.MapGet("/holidays/{countryCode}/{fromDate}/{toDate}",
            static (string countryCode, DateTime fromDate, DateTime toDate, CalendarService calendarService) =>
                GetHolidays(countryCode, fromDate, toDate, calendarService));
    }
    
    /// <summary>
    /// Gets the access control information of an employee associated with a specific ID.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="date">The date to detect holiday</param>
    /// <param name="calendarService">The service to handle holiday operations.</param>
    /// <returns>The holiday information.</returns>
    private static async Task<IResult> GetHoliday(string countryCode, DateTime date, CalendarService calendarService)
    {
        var holiday = await calendarService.GetHolidayAsync(countryCode, date);
        return holiday == null ? Results.Json(new { message = $"No holidays found for the specified date: '{date.ToShortDateString()}'" }, statusCode: 200) : Results.Json(holiday, statusCode: 200);
    }

    /// <summary>
    /// Gets the access control information of an employee associated with a specific ID.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="year">The year to detect holiday</param>
    /// <param name="calendarService">The service to handle holiday operations.</param>
    /// <returns>The holiday information.</returns>
    private static async Task<IResult> GetHolidays(string countryCode, int year, CalendarService calendarService)
    {
        var result = await calendarService.GetHolidaysAsync(countryCode, year);
        var holidays = result.ToList();
        return !holidays.Any() ? Results.Json(new { message = $"No holidays found in the specified year: '{year}'" }, statusCode: 200) : Results.Json(holidays, statusCode: 200);
    }

    /// <summary>
    /// Retrieves holidays within a specified date range.
    /// </summary>
    /// <param name="countryCode">In ISO 3166-1 alpha-2 format</param>
    /// <param name="fromDate">The start date of the range.</param>
    /// <param name="toDate">The end date of the range.</param>
    /// <param name="calendarService">The calendar service used to process the request.</param>
    /// <returns>A collection of holidays within the specified range.</returns>
    private static async Task<IResult> GetHolidays(string countryCode, DateTime fromDate, DateTime toDate, CalendarService calendarService)
    {
        // Retrieve holidays within the specified date range
        var result = await calendarService.GetHolidaysAsync(countryCode, fromDate, toDate);

        // Check if no holidays were found
        var holidays = result.ToList();
        return !holidays.Any()
            ? Results.Json(new { message = $"No holidays found in the specified date range: '{fromDate.ToShortDateString()}' - '{toDate.ToShortDateString()}'" }, statusCode: 200)
            : Results.Json(holidays, statusCode: 200);
    }
}
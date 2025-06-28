using Mycompany.Finance.Calendar;
using Mycompany.Finance.Calendar.Application.Health;
using Mycompany.Finance.Calendar.Endpoints;
using Prometheus;
using StackExchange.Redis;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// App services
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAuthorization();

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");
    if (string.IsNullOrEmpty(configuration))
        throw new ArgumentNullException($"Redis connection string is missing in configuration.");

    return ConnectionMultiplexer.Connect(configuration);
});

// Register culture settings
var cultureConfig = builder.Configuration.GetSection("CultureSettings");
var cultureInfo = new CultureInfo(cultureConfig["Culture"]!)
{
    DateTimeFormat =
    {
        ShortDatePattern = cultureConfig["ShortDatePattern"]!,
        LongDatePattern = cultureConfig["LongDatePattern"]!
    }
};
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Swagger
if (builder.Environment.IsDevelopment() ||
    builder.Environment.EnvironmentName.Equals("LocalDev", StringComparison.OrdinalIgnoreCase) ||
    builder.Environment.EnvironmentName.Equals("Test", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSwaggerConfiguration(builder.Configuration);
}

var app = builder.AddHealth().Build();

// --- Prometheus: expose /metrics endpoint ---
app.UseMetricServer(); // This will expose /metrics automatically
app.UseHttpMetrics();  // Collects default HTTP metrics for requests

// Configure middleware
app.UseMiddlewareConfiguration(app.Environment, builder.Configuration);
app.UseRouting();
app.UseAuthorization();
app.MapCalendarEndpoints();
app.UseHealth();
app.Run();
using Mycompany.Finance.Calendar;
using Mycompany.Finance.Calendar.Application.Health;
using Mycompany.Finance.Calendar.Endpoints;
using Mycompany.Finance.Calendar.Infrastructure;
using Prometheus;
using StackExchange.Redis;
using System.Globalization;

// Load API key from env var or secret file
string? apiKey = Environment.GetEnvironmentVariable("API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    var secretPath = Path.Combine(AppContext.BaseDirectory, "api_key.secret");
    if (File.Exists(secretPath))
    {
        apiKey = File.ReadAllText(secretPath).Trim();
    }
}

// Build configuration with the API key added
var configBuilder = new ConfigurationBuilder()
    .AddConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()) // optional
    .AddEnvironmentVariables();

if (!string.IsNullOrEmpty(apiKey))
{
    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
    {
        { "API_KEY", apiKey }
    });
}

var configuration = configBuilder.Build();

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddConfiguration(configuration);

// App services
builder.Services.AddServices(builder.Configuration);
builder.Services.AddAuthorization();

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnection = builder.Configuration.GetValue<string>("Redis:ConnectionString");
    if (string.IsNullOrEmpty(redisConnection))
        throw new ArgumentNullException("Redis connection string is missing in configuration.");

    return ConnectionMultiplexer.Connect(redisConnection);
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

// Prometheus
app.UseMetricServer();
app.UseHttpMetrics();

// Middleware
app.UseMiddleware<ApiKeyMiddleware>(); // Add your API key middleware here
app.UseMiddlewareConfiguration(app.Environment, builder.Configuration);
app.UseRouting();
app.UseAuthorization();
app.MapCalendarEndpoints();
app.UseHealth();

app.Run();

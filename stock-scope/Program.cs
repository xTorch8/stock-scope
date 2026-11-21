using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using StockScope.Clients;
using StockScope.Common;
using StockScope.Configurations;
using StockScope.Services;
using StockScope.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AlphaVantageConfiguration>(
    builder.Configuration.GetSection(AlphaVantageConfiguration.SectionName)
);

// Register Domain Services
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IFundamentalService, FundamentalService>();
builder.Services.AddScoped<ICommodityService, CommodityService>();
builder.Services.AddScoped<ITechnicalIndicatorService, TechnicalIndicatorService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<IAlphaVantageClient, AlphaVantageClient>((serviceProvider, client) =>
{
    var configuration = serviceProvider
        .GetRequiredService<IOptions<AlphaVantageConfiguration>>()
        .Value;

    if (!string.IsNullOrWhiteSpace(configuration.BaseUrl))
    {
        if (!Uri.TryCreate(configuration.BaseUrl, UriKind.Absolute, out var baseUri) ||
            baseUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                "ALPHA_VANTAGE:BaseUrl must be a valid HTTPS URL.");
        }

        client.BaseAddress = baseUri;
    }
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("mcp", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Add the MCP services: the transport to use (http) and the tools to register.
builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        // Stateless mode is recommended for servers that don't need
        // server-to-client requests like sampling or elicitation.
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    options.UseUtcTimestamp = false;
    options.SingleLine = true;
});
builder.Logging.AddDebug();
builder.Logging.AddProvider(new FileLoggerProvider(Path.Combine(builder.Environment.ContentRootPath, "logs")));
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

var app = builder.Build();
app.UseRateLimiter();
app.MapMcp().RequireRateLimiting("mcp");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.Run();

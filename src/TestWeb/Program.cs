using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.VngCloudLogging;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    // serilog config from app settings
    {
        // var configuration = new ConfigurationBuilder()
        //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //     .Build();
        //
        //  builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
    }
    // serilog config programmatically
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // Please follow document to get these settings
        var options = new VngCloudLoggingSinkOptions
        {
            BootstrapServers = "kafka_server",
            Topic = "sample_topic",
            SecurityProtocol = SecurityProtocol.Ssl,
            SslCaLocation = Path.Combine(currentDirectory, "VNG.trust.pem"),
            SslCertificateLocation = Path.Combine(currentDirectory, "user.cer.pem"),
            SslKeyLocation = Path.Combine(currentDirectory, "user.key.pem")
        };

        builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().WriteTo.VngCloudLogging(options).MinimumLevel.Is(LogEventLevel.Verbose));
    }

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    app.MapGet("/", ([FromServices] ILogger<Program> _logger, [FromServices] ILoggerFactory _loggerFactory) =>
    {
        Log.Information("Test info message with serilog");
        Log.Debug("Test debug message with serilog");

        _logger.LogInformation("Test info message with ILogger abstraction");
        _logger.LogDebug("Test debug message with ILogger abstraction");

        var anotherLogger = _loggerFactory.CreateLogger("AnotherLogger");
        anotherLogger.LogInformation("Test message with ILoggerFactory abstraction and custom log name");

        _logger.LogInformation(eventId: new Random().Next(), message: "Test message with random event ID");
        _logger.LogInformation("Test message with List<string> {list}", new List<string> { "foo", "bar", "pizza" });
        _logger.LogInformation("Test message with List<int> {list}", new List<int> { 123, 456, 7890 });
        _logger.LogInformation("Test message with Dictionary<string,object> {dict}", new Dictionary<string, object>
            {
                { "valueAsNull", null },
                { "valueAsBool", true },
                { "valueAsString", "qwerty" },
                { "valueAsStringNumber", "00000" },
                { "valueAsMaxInt", int.MaxValue },
                { "valueAsMaxLong", long.MaxValue },
                { "valueAsDouble", 123.456 },
                { "valueAsMaxDouble", double.MaxValue },
                { "valueAsMaxDecimal", decimal.MaxValue },
            });

        try
        {
            throw new Exception("Testing exception logging");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "");
        }

        var url = $"https://vmonitor.console.vngcloud.vn/log/project";
        var html = $"<html><body>Logged messages at {DateTime.UtcNow:O}, visit Log Project at <a href='{url}' target='_blank'>{url}</a></body></html>";

        return Results.Content(html, "text/html");
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
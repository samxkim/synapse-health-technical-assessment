using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Exceptions;

namespace OrderApi.Startup;

public static class SeriLogConfiguration
{
    public static void RegisterLogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
            });

        Log.Logger = GetLogger(builder.Configuration);
    }

    public static Serilog.ILogger GetLogger(ConfigurationManager configurationManager)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .ReadFrom.Configuration(configurationManager)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .WriteTo.ApplicationInsights(new TelemetryConfiguration
            {
                // InstrumentationKey = configurationManager["Monitoring:AzureApplicationInsightsInstrumentationKey"]
            }, TelemetryConverter.Traces);

        Log.Logger = loggerConfiguration.CreateLogger();
        return Log.Logger;
    }
}
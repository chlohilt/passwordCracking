using System.Text;
using Microsoft.Extensions.Logging;

internal static class Settings {

    /// <summary>
    /// Service provider used to retrieve a logger with <see cref="ServiceProvider.GetService(Type)" />
    /// </summary>
    /// <value></value>
    public static ILoggerFactory LoggerFactory { get; private set; }

    /// <summary>
    /// The log level to display in the program.
    /// 
    /// Change to <see cref="LogLevel.Debug" /> to see additional debugging messages.
    /// </summary>
    private static LogLevel MinimumLogLevel { get => LogLevel.Information; }

    /// <summary>
    /// Encoding used to convert strings to and from bytes.
    /// </summary>
    public static Encoding Encoding { get => Encoding.ASCII; }

    /// <summary>
    /// Sets up the service provider.
    /// </summary>
    static Settings() {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => {
            builder
                .SetMinimumLevel(MinimumLogLevel)
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddSimpleConsole(options => {
                    options.TimestampFormat = "HH:mm:ss ";
                    options.SingleLine = true;
                    options.IncludeScopes = true;
                });
        });
    }

}
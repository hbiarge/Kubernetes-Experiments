using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mvc.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Mvc
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration((context, builder) =>
                        {
                            // Reload doesn't work in this scenario because kubernetes mount the file
                            // as a symlink, and the modified date of the symlink is not updated 
                            // when the underlying file changes. The FileProvider relies in that date
                            // to notify the configuration system about changes. So it does not work :(
                            // See: https://github.com/dotnet/extensions/issues/1175
                            // Here we use a custom implementation that relies in file content checksum
                            // to notify changes. Use with caution. Not tested...
                            // See: https://github.com/fbeltrao/ConfigMapFileProvider
                            builder.AddJsonFile(
                                ConfigMapFileProvider.FromRelativePath("config"),
                                "config.json", 
                                optional: true, 
                                reloadOnChange: true);
                            builder.AddJsonFile(
                                ConfigMapFileProvider.FromRelativePath("secret"),
                                "secret.json", 
                                optional: true, 
                                reloadOnChange: true);
                        })
                        .ConfigureLogging((context, builder) =>
                        {
                            var logger = new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .MinimumLevel.Override(
                                    "Microsoft", 
                                    LogEventLevel.Warning)
                                .MinimumLevel.Override(
                                    "System", 
                                    LogEventLevel.Warning)
                                .Enrich.FromLogContext()
                                .WriteTo.Console(
                                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}",
                                    theme: AnsiConsoleTheme.Literate)
                                .CreateLogger();

                            builder.ClearProviders();
                            builder.AddSerilog(logger, dispose: true);
                        })
                        .UseStartup<Startup>();
                });
    }
}

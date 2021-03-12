using System.IO;
using System.Reflection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    internal static class LoggingExtensions
    {
        internal static void ConfigureLogging(this IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(logBuilder =>
            {
                // all logging is filtered out by default
                logBuilder.AddFilter("SFA.DAS", LogLevel.Information);
                var rootDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), ".."));
                var files = Directory.GetFiles(rootDirectory, "nlog.config", SearchOption.AllDirectories)[0];
                logBuilder.AddNLog(files);
            });
        }
    }
}

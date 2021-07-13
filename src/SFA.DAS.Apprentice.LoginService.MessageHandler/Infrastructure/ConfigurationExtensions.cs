using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class ConfigurationExtensions
    {
        internal static void ConfigureConfiguration(this IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true);

            var preConfig = builder.ConfigurationBuilder.Build();

            builder.ConfigurationBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = preConfig["ConfigNames"].Split(",");
                options.StorageConnectionString = preConfig["ConfigurationStorageConnectionString"];
                options.EnvironmentName = preConfig["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }
        
        public static bool IsAcceptanceTest(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsLocalOrDev(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsLocalAcceptanceOrDev(this IConfiguration config)
        {
            return config.IsLocalOrDev() || config.IsAcceptanceTest();
        }
    }
}

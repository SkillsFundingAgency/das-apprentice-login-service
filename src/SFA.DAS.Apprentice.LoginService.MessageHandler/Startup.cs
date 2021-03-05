using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Http.Configuration;
using SFA.DAS.LoginService.Configuration;

[assembly: FunctionsStartup(typeof(SFA.DAS.Apprentice.LoginService.MessageHandler.Startup))]

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true);

            var preConfig = builder.ConfigurationBuilder.Build();

            builder.ConfigurationBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = preConfig["ConfigNames"].Split(",");
                options.StorageConnectionString = preConfig["ConfigurationStorageConnectionString"];
                options.EnvironmentName = preConfig["EnvironmentName"];
                options.PreFixConfigurationKeys = true;
            });
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLogging();
            builder.ConfigureNServiceBus();

            builder.Services
                .AddOptions<LoginConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind("SFA.DAS.ApprenticeLoginService", settings));

            builder.Services.AddSingleton<ILoginConfig>(s =>
                s.GetRequiredService<IOptions<LoginConfig>>().Value);

            var sp = builder.Services.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var login = sp.GetRequiredService<ILoginConfig>();

            builder.Services.AddInvitationService(configuration, login.ApprenticeLoginApi);
        }
    }
}

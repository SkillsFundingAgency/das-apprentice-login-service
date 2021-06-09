using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.LoginService.Configuration;

[assembly: FunctionsStartup(typeof(SFA.DAS.Apprentice.LoginService.MessageHandler.Startup))]

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
            => builder.ConfigureConfiguration();

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLogging();

            var logger = LoggerFactory.Create(b => b.ConfigureLogging()).CreateLogger<Startup>();
            AutoSubscribeToQueues.CreateQueuesWithReflection(
                builder.GetContext().Configuration,
                connectionStringName: "NServiceBusConnectionString",
                logger: logger)
                .GetAwaiter().GetResult();

            builder.UseNServiceBus(() =>
            {
                var configuration = new ServiceBusTriggeredEndpointConfiguration(
                    endpointName: QueueNames.ApprenticeLoginService,
                    connectionStringName: "NServiceBusConnectionString");

                configuration.AdvancedConfiguration.Conventions()                    
                    .DefiningMessagesAs(t => t.Namespace?.StartsWith("SFA.DAS.Apprentice.LoginService.Messages.Commands") == true);                    

                configuration.AdvancedConfiguration.SendFailedMessagesTo($"{QueueNames.ApprenticeLoginService}-error");
                configuration.LogDiagnostics();

                configuration.Transport.SubscriptionRuleNamingConvention(AzureQueueNameShortener.Shorten);

                return configuration;
            });
            builder.Services
                .AddOptions<LoginConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind("SFA.DAS.ApprenticeLoginService", settings));

            builder.Services.AddSingleton<ILoginConfig>(s =>
                s.GetRequiredService<IOptions<LoginConfig>>().Value);

            var sp = builder.Services.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var login = sp.GetRequiredService<ILoginConfig>();

            login.ApprenticeLoginApi ??= new ApprenticeLoginApiConfiguration { ApiBaseUrl = "https://localhost:5001" };

            builder.Services.AddInvitationService(configuration, login.ApprenticeLoginApi);
        }
    }
}
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;

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
                    .DefiningMessagesAs(t => t.Namespace?.StartsWith("SFA.DAS.Apprentice.LoginService.Messages") == true);

                configuration.AdvancedConfiguration.SendFailedMessagesTo($"{QueueNames.ApprenticeLoginService}-error");
                configuration.LogDiagnostics();

                configuration.Transport.SubscriptionRuleNamingConvention(AzureQueueNameShortener.Shorten);

                return configuration;
            });

            builder.Services.AddSingleton(p =>
                p.GetRequiredService<IConfiguration>().Get<LoginConfig>());

            builder.Services.AddSingleton<ILoginConfig>(p =>
                p.GetRequiredService<LoginConfig>());

            builder.Services.AddScoped<IMessageSession, FunctionEndpointSessionAdaptor>();

            builder.Services.WireUpDependencies(builder.GetContext());

            builder.Services.AddDbContext<LoginContext>((services, options) =>
                services.GetRequiredService<IContextSecurityProvider>().Secure(options));

            builder.Services.AddDbContext<LoginUserContext>((services, options) =>
                services.GetRequiredService<IContextSecurityProvider>().Secure(options));

            //builder.Services.AddIdentityServer(_loginConfig, _environment, _logger);
        }
    }
}
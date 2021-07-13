using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;
using SFA.DAS.Notifications.Messages.Commands;
using System;

[assembly: FunctionsStartup(typeof(SFA.DAS.Apprentice.LoginService.MessageHandler.Startup))]

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class Startup : FunctionsStartup
    {
        private const string NotificationsEndpointName = "SFA.DAS.Notifications.MessageHandlers";

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

                configuration.AdvancedConfiguration.Conventions()
                    .DefiningMessagesAs(IsMessage)
                    .DefiningEventsAs(IsEvent)
                    .DefiningCommandsAs(IsCommand);

                configuration.Transport.SubscriptionRuleNamingConvention(AzureQueueNameShortener.Shorten);

                configuration.Transport.Routing().RouteToEndpoint(typeof(SendEmailCommand), NotificationsEndpointName);

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
        }

        private static bool IsMessage(Type t) => IsSfaMessage(t, "Messages");

        private static bool IsEvent(Type t) => IsSfaMessage(t, "Messages.Events");

        private static bool IsCommand(Type t) => IsSfaMessage(t, "Messages.Commands");

        private static bool IsSfaMessage(Type t, string namespaceSuffix)
            => t.Namespace != null &&
                t.Namespace.StartsWith("SFA.DAS") &&
                t.Namespace.EndsWith(namespaceSuffix);
    }
}
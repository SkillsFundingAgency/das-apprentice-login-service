using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Configuration.NLog;
using SFA.DAS.NServiceBus.Hosting;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public static class NServiceBusStartupExtensions
    {
        private const string EndpointName = "SFA.Apprentice.Login";
        private const string NotificationsEndpointName = "SFA.DAS.Notifications.MessageHandlers";
        private const string ApprenticeAccountsEndpointName = "SFA-DAS-ApprenticeAccounts";

        public static IServiceCollection AddNServiceBus(this IServiceCollection services)
        {
            return services
                .AddSingleton(p =>
                {
                    var sp = services.BuildServiceProvider();
                    var configuration = sp.GetService<NServiceBusConfiguration>();

                    try
                    {
                        var hostingEnvironment = p.GetService<IWebHostEnvironment>();

                        var endpointConfiguration = new EndpointConfiguration(EndpointName)
                            .UseLicense(configuration.NServiceBusLicense)
                            .UseMessageConventions()
                            .UseNewtonsoftJsonSerializer()
                            .UseNLogFactory();

                        endpointConfiguration.SendOnly();

                        if (hostingEnvironment.IsDevelopment())
                            endpointConfiguration.UseLearningTransport(s => s.AddRouting());
                        else
                            endpointConfiguration.UseAzureServiceBusTransport(configuration.SharedServiceBusEndpointUrl, s => s.AddRouting());

                        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

                        return endpoint;
                    }
                    catch (System.Exception e)
                    {
                        throw new System.Exception($"Failed to start NServiceBus for endpoint `{configuration?.SharedServiceBusEndpointUrl}`", e);
                    }
                })
                .AddSingleton<IMessageSession>(s => s.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }

        public static void AddRouting(this RoutingSettings routingSettings)
        {
            routingSettings.RouteToEndpoint(typeof(SendEmailCommand), NotificationsEndpointName);
            routingSettings.RouteToEndpoint(typeof(UpdateEmailAddressCommand), ApprenticeAccountsEndpointName);
        }

        public static EndpointConfiguration UseLicense(this EndpointConfiguration config, string licenseText)
        {
            if (licenseText != null)
                config.License(licenseText);
            return config;
        }
    }
}
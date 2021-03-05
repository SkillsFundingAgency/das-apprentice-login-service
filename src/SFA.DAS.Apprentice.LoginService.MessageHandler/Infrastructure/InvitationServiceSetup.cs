using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestEase.HttpClientFactory;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.LoginService.Configuration;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class InvitationServiceSetup
    {
        public static IServiceCollection AddInvitationService(this IServiceCollection serviceCollection, IConfiguration configuration, ApprenticeLoginApiConfiguration apiConfig)
        {
            serviceCollection.AddTransient<IManagedIdentityTokenGenerator>(_ => new ManagedIdentityTokenGenerator(apiConfig));

            serviceCollection.AddTransient<Http.MessageHandlers.DefaultHeadersHandler>();
            serviceCollection.AddTransient<Http.MessageHandlers.LoggingMessageHandler>();
            serviceCollection.AddTransient<Http.MessageHandlers.ManagedIdentityHeadersHandler>();

            var apiClient = serviceCollection.AddRestEaseClient<IInvitationApi>(apiConfig.ApiBaseUrl)
                .AddHttpMessageHandler<Http.MessageHandlers.DefaultHeadersHandler>();

            if (!configuration.IsLocalAcceptanceOrDev())
            {
                apiClient.AddHttpMessageHandler<Http.MessageHandlers.ManagedIdentityHeadersHandler>();
            }
            apiClient.AddHttpMessageHandler<Http.MessageHandlers.LoggingMessageHandler>();

            return serviceCollection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services
{
    public class TestMessageBus
    {
        private IEndpointInstance endpointInstance;
        public bool IsRunning { get; private set; }
        public DirectoryInfo StorageDirectory { get; private set; }

        public async Task Start()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.ApprenticeLoginService.MessageHandler.TestMessageBus");
            endpointConfiguration
                .UseNewtonsoftJsonSerializer()
                .UseMessageConventions()
                .UseTransport<LearningTransport>();

            endpointConfiguration.UseLearningTransport(s => s.RouteToEndpoint(typeof(SendInvitationCommand), QueueNames.SendInvitationCommand));

            endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            IsRunning = true;
        }

        public async Task Stop()
        {
            await endpointInstance.Stop();
            IsRunning = false;
        }

        public Task Publish(object message) => endpointInstance.Publish(message);

        public async Task Send(object message)
        {
            await endpointInstance.Send(message);
        }
    }

    public class MessageBusHook<T> : IHook
    {
        public Action<T> OnReceived { get; set; }
        public Action<T> OnProcessed { get; set; }
        public Action<Exception, T> OnErrored { get; set; }
    }
}
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using System.Threading.Tasks;

namespace SFASFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus
{
    internal class AzureServiceBusTriggerFunction
    {
        private const string EndpointName = QueueNames.ApprenticeLoginService;
        private readonly IFunctionEndpoint endpoint;

        public AzureServiceBusTriggerFunction(IFunctionEndpoint endpoint) => this.endpoint = endpoint;

        [FunctionName("AzureServiceBusTrigger")]
        public async Task Run(
            [ServiceBusTrigger(queueName: EndpointName, Connection = "NServiceBusConnectionString")] Message message,
            ILogger logger,
            ExecutionContext context)
        {
            await endpoint.Process(message, context, logger);
        }
    }
}
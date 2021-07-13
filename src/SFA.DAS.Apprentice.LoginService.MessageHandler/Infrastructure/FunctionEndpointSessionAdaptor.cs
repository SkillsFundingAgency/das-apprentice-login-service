using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    internal class FunctionEndpointSessionAdaptor : IMessageSession
    {
        private readonly IFunctionEndpoint endpoint;
        private readonly ExecutionContext context;
        private readonly ILogger logger;

        public FunctionEndpointSessionAdaptor(
            IFunctionEndpoint functionEndpoint,
            ExecutionContext executionContext,
            ILogger functionsLogger)
        {
            endpoint = functionEndpoint;
            context = executionContext;
            logger = functionsLogger;
        }

        public Task Publish(object message, PublishOptions options)
            => endpoint.Publish(message, options, context, logger);

        public Task Publish<T>(Action<T> messageConstructor, PublishOptions publishOptions)
            => endpoint.Publish(messageConstructor, publishOptions, context, logger);

        public Task Send(object message, SendOptions options)
            => endpoint.Send(message, options, context, logger);

        public Task Send<T>(Action<T> messageConstructor, SendOptions options)
            => endpoint.Send(messageConstructor, options, context, logger);

        public Task Subscribe(Type eventType, SubscribeOptions options)
            => endpoint.Subscribe(eventType, options, context, logger);

        public Task Unsubscribe(Type eventType, UnsubscribeOptions options)
            => endpoint.Unsubscribe(eventType, options, context, logger);
    }
}
﻿using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNServiceBus<T>(
            this IServiceCollection serviceCollection,
            Action<NServiceBusOptions> OnConfigureOptions = null)
        {
            serviceCollection.AddSingleton<IExtensionConfigProvider, NServiceBusExtensionConfigProvider>(services =>
            {
                var logger = services.GetRequiredService<ILogger<T>>();

                var options = new NServiceBusOptions
                {
                    OnMessageReceived = (context) =>
                    {
                        context.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out string messageType);
                        context.Headers.TryGetValue("NServiceBus.MessageId", out string messageId);
                        context.Headers.TryGetValue("NServiceBus.CorrelationId", out string correlationId);
                        context.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out string originatingEndpoint);
                        logger.LogInformation($"Received NServiceBusTriggerData Message of type '{messageType.SimpleName()}' " +
                            $"with messageId '{messageId}' and correlationId '{correlationId}' from endpoint '{originatingEndpoint}'");
                    },
                    OnMessageErrored = (_, context) =>
                    {
                        context.Headers.TryGetValue("NServiceBus.EnclosedMessageTypes", out string messageType);
                        context.Headers.TryGetValue("NServiceBus.MessageId", out string messageId);
                        context.Headers.TryGetValue("NServiceBus.CorrelationId", out string correlationId);
                        context.Headers.TryGetValue("NServiceBus.OriginatingEndpoint", out string originatingEndpoint);
                        logger.LogInformation($"Error handling Message of type '{messageType.SimpleName()}' " +
                            $"with messageId '{messageId}' and correlationId '{correlationId}' from endpoint '{originatingEndpoint}'");
                    }
                };

                OnConfigureOptions?.Invoke(options);

                return new NServiceBusExtensionConfigProvider(options);
            });

            return serviceCollection;
        }

        private static string SimpleName(this string commaSeparated)
            => commaSeparated?.Split(',').FirstOrDefault()
            ?? commaSeparated.Substring(0, Math.Min(30, commaSeparated.Length));
    }
}

﻿using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus
{
    internal static class FunctionsHostBuilderExtension
    {
        internal static void ConfigureNServiceBus(this IFunctionsHostBuilder builder)
        {
            var config = builder.GetNServiceBusConfiguration();
            builder.Services.AddNServiceBus<Startup>(config);
        }

        private static Action<NServiceBusOptions> GetNServiceBusConfiguration(
            this IFunctionsHostBuilder builder)
        {
            var configuration = builder.Services
                .BuildServiceProvider()
                .GetService<IConfiguration>();

            return configuration["NServiceBusConnectionString"] == "UseDevelopmentStorage=true"
                ? (Action<NServiceBusOptions>)ConfigureForDevelopment
                : (Action<NServiceBusOptions>)ConfigureForReal;
        }

        private static void ConfigureForDevelopment(NServiceBusOptions options)
        {
            options.EndpointConfiguration = (endpoint) =>
            {
                endpoint
                    .UseTransport<LearningTransport>()
                    .StorageDirectory(".learningtransport");
                return endpoint;
            };
        }

        private static void ConfigureForReal(NServiceBusOptions _)
        { }
    }
}

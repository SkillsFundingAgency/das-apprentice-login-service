﻿using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LoginService.Configuration;
using StackExchange.Redis;
using System;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public static class DataProtectionStartupExtensions
    {
        public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment, IServiceProvider serviceProvider)
        {
            if (!environment.IsDevelopment())
            {
                ILoginConfig loginConfig = new ConfigurationService(serviceProvider.GetService<IMediator>())
                .GetLoginConfig(
                    configuration["EnvironmentName"],
                    configuration["ConfigurationStorageConnectionString"],
                    "1.0",
                    "SFA.DAS.ApprenticeLoginService", environment).Result;

                if (loginConfig != null)
                {
                    var redisConnectionString = loginConfig.RedisConnectionString;
                    var dataProtectionKeysDatabase = loginConfig.DataProtectionKeysDatabase;

                    var redis = ConnectionMultiplexer
                        .Connect($"{redisConnectionString},{dataProtectionKeysDatabase}");

                    services.AddDataProtection()
                        .SetApplicationName("das-apprentice-login-service")
                        .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
                }
            }
            return services;
        }
    }
}

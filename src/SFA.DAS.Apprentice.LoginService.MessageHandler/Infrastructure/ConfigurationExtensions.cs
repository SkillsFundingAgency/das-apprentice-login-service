﻿using System;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class ConfigurationExtensions
    {
        public static bool IsAcceptanceTest(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsLocalOrDev(this IConfiguration config)
        {
            return config["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                   config["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsLocalAcceptanceOrDev(this IConfiguration config)
        {
            return config.IsLocalOrDev() || config.IsAcceptanceTest();
        }
    }
}

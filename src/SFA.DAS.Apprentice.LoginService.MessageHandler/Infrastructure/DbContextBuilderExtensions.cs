using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class DbContextBuilderExtensions
    {
        public static DbContextOptionsBuilder UseDataStorage(
            this DbContextOptionsBuilder builder, IConnectionFactory connectionFactory, string connection)
        {
            return connectionFactory.AddConnection(builder, connection);
        }

        public static DbContextOptionsBuilder UseLocalSqlLogger<TContext>(
            this DbContextOptionsBuilder builder, ILoggerFactory loggerFactory, IConfiguration config)
        {
            if (config.IsLocalAcceptanceOrDev())
            {
                builder.EnableSensitiveDataLogging().UseLoggerFactory(loggerFactory);
            }

            return builder;
        }
    }
}

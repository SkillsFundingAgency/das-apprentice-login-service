using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.LoginService.Configuration;
using System.Data.SqlClient;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public interface IContextSecurityProvider
    {
        DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder);
    }

    public class ManagedIdentityAzureTokenProvider : IContextSecurityProvider
    {
        private readonly LoginConfig _config;

        public ManagedIdentityAzureTokenProvider(LoginConfig config)
            => _config = config;

        public DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder)
        {
            var sqlConnection = new SqlConnection(_config.SqlConnectionString)
            {
                AccessToken = GetAccessToken()
            };

            builder.UseSqlServer(sqlConnection);

            return builder;
        }

        private string GetAccessToken()
        {
            return new AzureServiceTokenProvider()
                .GetAccessTokenAsync("https://database.windows.net/")
                .GetAwaiter().GetResult();
        }
    }

    public class DevSecurityProvider : IContextSecurityProvider
    {
        private readonly LoginConfig _config;

        public DevSecurityProvider(LoginConfig config) => _config = config;

        public DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder)
        {
            return builder.UseSqlServer(_config.SqlConnectionString);
        }
    }
}
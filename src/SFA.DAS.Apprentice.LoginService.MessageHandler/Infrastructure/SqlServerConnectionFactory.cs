using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public class SqlServerConnectionFactory : IConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public SqlServerConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbContextOptionsBuilder AddConnection(DbContextOptionsBuilder builder, string connection)
        {
            return builder.UseSqlServer(CreateConnection(connection));
        }

        public DbConnection CreateConnection(string connection)
        {
            var sqlConnection = new SqlConnection(connection)
            {
                AccessToken = GetAccessToken(),
            };

            return sqlConnection;
        }

        private string GetAccessToken()
        {
            if (_configuration.IsLocalAcceptanceOrDev())
            {
                return null;
            }

            return new AzureServiceTokenProvider()
                .GetAccessTokenAsync("https://database.windows.net/")
                .GetAwaiter().GetResult();
        }
    }
}

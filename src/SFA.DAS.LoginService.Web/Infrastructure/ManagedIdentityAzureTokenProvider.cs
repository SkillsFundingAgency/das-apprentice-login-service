using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Data.SqlClient;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public interface IContextSecurityProvider
    {
        DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder, string sqlConnectionString);
    }

    public class ManagedIdentityAzureTokenProvider : IContextSecurityProvider
    {
        private readonly IWebHostEnvironment _environment;

        public ManagedIdentityAzureTokenProvider(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder, string sqlConnectionString)
        {
            var sqlConnection = new SqlConnection(sqlConnectionString);

            if (!_environment.IsDevelopment())
            {
                sqlConnection.AccessToken = GetAccessToken();
            }

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
}
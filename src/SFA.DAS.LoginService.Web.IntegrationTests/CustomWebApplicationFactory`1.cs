using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LoginService.Web.Infrastructure;

namespace SFA.DAS.LoginService.Web.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
                    services.AddSingleton<IContextSecurityProvider>(_ =>
                             new StubManagedIdentityProvider()));
        }
    }

    public class StubManagedIdentityProvider : IContextSecurityProvider
    {
        public DbContextOptionsBuilder Secure(DbContextOptionsBuilder builder, string sqlConnectionString)
        {
            return builder;
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
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
                    services.AddSingleton<IManagedIdentityTokenProvider>(_ =>
                             new StubManagedIdentityProvider()));
        }
    }

    public class StubManagedIdentityProvider : IManagedIdentityTokenProvider
    {
        public string GetAccessToken() => null;
    }
}
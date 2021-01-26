using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SFA.DAS.LoginService.Web.IntegrationTests.Login
{
    [TestFixture]
    public class When_Login_GET_called
    {
        [Test]
        public async Task Then_NotFound_is_not_returned()
        {
            var client = new CustomWebApplicationFactory<Startup>().CreateClient();

            var response = await client.GetAsync("/Account/Login");

            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }
    }
}
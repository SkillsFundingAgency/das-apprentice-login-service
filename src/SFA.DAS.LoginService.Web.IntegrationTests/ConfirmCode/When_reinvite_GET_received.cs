using FluentAssertions;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.IntegrationTests.ConfirmCode
{
    [TestFixture]
    public class When_reinvite_GET_received
    {
        [Test]
        public async Task Then_404_NotFound_is_not_returned()
        {
            var client = new CustomWebApplicationFactory<Startup>().CreateClient();

            var response = await client.GetAsync("/Invitations/Reinvite/" + Guid.NewGuid());

            response.StatusCode.Should().NotBe(HttpStatusCode.NotFound);
        }
    }
}

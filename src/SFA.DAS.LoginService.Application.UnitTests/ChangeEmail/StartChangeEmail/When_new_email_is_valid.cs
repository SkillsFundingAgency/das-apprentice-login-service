using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.StartChangeEmail
{
    public class When_new_email_is_valid : CreateStartChangeEmailHandlerTestBase
    {
        [Test]
        public async Task And_user_is_not_found()
        {
            var request = CreateStartChangeEmailRequest();
            request.CurrentEmailAddress = "UnknownUser@test.com";
            try
            {
                await Sut.Handle(request, CancellationToken.None);
                Assert.Fail();
            }
            catch (ApplicationException e)
            {
                e.Message.Should().Contain($"{request.CurrentEmailAddress} does not exist");
            }
        }
    }
}
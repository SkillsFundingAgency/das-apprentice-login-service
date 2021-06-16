using FluentAssertions;
using Microsoft.Extensions.Logging;
using NServiceBus;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;
using SFA.DAS.Notifications.Messages.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.Services.EmailServiceTests
{
    public class When_sending_change_user_email
    {
        [Test]
        public async Task Then_message_is_sent()
        {
            var messageSession = Substitute.For<IMessageSession>();
            var logger = Substitute.For<ILogger<EmailService.EmailService>>();
            var emailService = new EmailService.EmailService(messageSession, logger);

            await emailService.SendChangeEmailCode(new ChangeUserEmailViewModel
            {
                EmailAddress = "emailaddress@example.com",
                GivenName = "bobby",
                FamilyName = "roberts",
            });

            await messageSession.Received().Send(
                Verify.That<SendEmailCommand>(cmd =>
                cmd.Should().BeEquivalentTo(new
                {
                    RecipientsAddress = "emailaddress@example.com",
                    Tokens = new Dictionary<string, string>
                    {
                        {"EmailAddress", "emailaddress@example.com" },
                        {"GivenName", "bobby" },
                        {"FamilyName", "roberts" },
                    },
                })),
                Arg.Any<SendOptions>());
        }
    }
}

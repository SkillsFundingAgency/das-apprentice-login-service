using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.StartChangeEmail
{
    public class When_the_new_email_details_are_invalid : CreateStartChangeEmailHandlerTestBase
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public async Task Because_new_email_is_blank(string newEmail)
        {
            var request = CreateStartChangeEmailRequest();
            request.NewEmailAddress = newEmail;
            var response = await Handler.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public async Task Because_confirmed_email_is_blank(string confirmedEmail)
        {
            var request = CreateStartChangeEmailRequest();
            request.ConfirmEmailAddress = confirmedEmail;
            var response = await Handler.Handle(request, CancellationToken.None);
            response.ConfirmEmailAddressError.Should().NotBeEmpty();
        }

        [TestCase("A")]
        [TestCase("A@")]
        [TestCase("A@.")]
        [TestCase("A/@.com")]
        [TestCase("A@as@.com")]
        public async Task Because_new_email_is_not_valid(string newEmail)
        {
            var request = CreateStartChangeEmailRequest();
            request.NewEmailAddress = newEmail;
            var response = await Handler.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
        }

        [TestCase("A")]
        [TestCase("A@")]
        [TestCase("A@.")]
        [TestCase("A/@.com")]
        [TestCase("A@as@.com")]
        public async Task Because_confirm_email_is_not_valid(string confirmEmail)
        {
            var request = CreateStartChangeEmailRequest();
            request.ConfirmEmailAddress = confirmEmail;
            var response = await Handler.Handle(request, CancellationToken.None);
            response.ConfirmEmailAddressError.Should().NotBeEmpty();
        }

        [Test]
        public async Task Because_new_email_matches_current_email()
        {
            var request = CreateStartChangeEmailRequest();
            request.CurrentEmailAddress = request.NewEmailAddress;
            var response = await Handler.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
        }
    }
}
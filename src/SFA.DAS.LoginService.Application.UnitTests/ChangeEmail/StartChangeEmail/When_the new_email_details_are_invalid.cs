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
            var response = await Sut.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
            response.NewEmailAddressError.Should().Be("Email address cannot be blank");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public async Task Because_confirmed_email_is_blank(string confirmedEmail)
        {
            var request = CreateStartChangeEmailRequest();
            request.ConfirmEmailAddress = confirmedEmail;
            var response = await Sut.Handle(request, CancellationToken.None);
            response.ConfirmEmailAddressError.Should().NotBeEmpty();
            response.ConfirmEmailAddressError.Should().Be("Email address cannot be blank");
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
            var response = await Sut.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
            response.NewEmailAddressError.Should().Be("Must be a valid email address");
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
            var response = await Sut.Handle(request, CancellationToken.None);
            response.ConfirmEmailAddressError.Should().NotBeEmpty();
            response.ConfirmEmailAddressError.Should().Be("Must be a valid email address");
        }

        [Test]
        public async Task Because_new_email_does_not_match_confirm_email()
        {
            var request = CreateStartChangeEmailRequest();
            request.ConfirmEmailAddress = "XX" + request.NewEmailAddress;
            var response = await Sut.Handle(request, CancellationToken.None);
            response.ConfirmEmailAddressError.Should().NotBeEmpty();
            response.ConfirmEmailAddressError.Should().Be("Email addresses must match");
        }

        [Test]
        public async Task Because_new_email_matches_current_email()
        {
            var request = CreateStartChangeEmailRequest();
            User.Email = request.NewEmailAddress;
            var response = await Sut.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
            response.NewEmailAddressError.Should().Be("This email is the same as your current email address");
        }

        [Test]
        public async Task Because_new_email_matches_another_existing_email()
        {
            var request = CreateStartChangeEmailRequest();
            User.Email = CurrentUserEmail;
            request.NewEmailAddress = AnotherUserEmail;
            request.ConfirmEmailAddress = AnotherUserEmail;

            var response = await Sut.Handle(request, CancellationToken.None);
            response.NewEmailAddressError.Should().NotBeEmpty();
            response.NewEmailAddressError.Should().Be("This email is already in use by another account");
        }
    }
}
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NServiceBus;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class When_ConfirmChangeEmail_called_for_existing_user_with_valid_token_and_password : ConfirmChangeEmailBase
    {
        [SetUp]
        public void Setup()
        {
            _userManager.CheckPasswordAsync(_user, _request.Password)
                .Returns(true);

            _userManager.ChangeEmailAsync(default, default, default)
                .ReturnsForAnyArgs(IdentityResult.Success);

            _userManager.SetUserNameAsync(_user, _request.NewEmailAddress)
                .Returns(IdentityResult.Success);
        }

        [Test]
        public async Task Then_finds_user_based_on_current_email_address()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _userManager.Received()
                .FindByIdAsync(_request.SubjectId);
        }

        [Test]
        public async Task Then_changes_the_email_address()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _userManager.Received()
                .ChangeEmailAsync(_user, _request.NewEmailAddress, _request.Token);
        }

        [Test]
        public async Task Then_updated_the_username_to_match_the_email_address()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _userManager.Received()
                .SetUserNameAsync(_user, _request.NewEmailAddress);
        }

        [Test]
        public async Task Then_returns_no_errors()
        {
            var result = await _sut.Handle(_request, CancellationToken.None);
            result.Should().BeEquivalentTo(new
            {
                PasswordError = (string)null,
                TokenError = false,
            });
        }

        [Test]
        public async Task Then_publishes_event_EmailChangedEvent()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _messageSession.Received()
                .Send(Arg.Is<UpdateEmailAddressCommand>(m =>
                    m.ApprenticeId == _user.ApprenticeId && m.NewEmailAddress == _request.NewEmailAddress &&
                    m.CurrentEmailAddress == _request.CurrentEmailAddress), Arg.Any<SendOptions>());
        }
    }
}
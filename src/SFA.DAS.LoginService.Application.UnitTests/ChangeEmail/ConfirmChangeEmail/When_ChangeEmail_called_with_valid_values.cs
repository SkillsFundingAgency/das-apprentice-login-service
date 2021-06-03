using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NUnit.Framework;
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
            _userService
                .ChangeEmail(default, default, default, default)
                .ReturnsForAnyArgs(IdentityResult.Success);
        }

        [Test]
        public async Task Then_finds_user_based_on_current_email_address()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _userService.Received()
                .FindByEmail(_request.CurrentEmailAddress);
        }

        [Test]
        public async Task Then_passes_form_values_to_ChangeEmail()
        {
            await _sut.Handle(_request, CancellationToken.None);

            await _userService.Received()
                .ChangeEmail(_user, _request.Password, _request.NewEmailAddress, _request.Token);
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
    }
}
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class When_ConfirmChangeEmail_called_for_existing_user_with_blank_password : ConfirmChangeEmailBase
    {
        [SetUp]
        public void Setup()
        {
            _request.Password = "";
        }

        [Test]
        public async Task Then_returns_token_errors()
        {
            var result = await _sut.Handle(_request, CancellationToken.None);
            result.Should().BeEquivalentTo(new
            {
                PasswordError = "Password cannot be blank",
                TokenError = false,
            });
        }

        [Test]
        public async Task Then_does_not_attempt_to_change_email()
        {
            await _sut.Handle(_request, CancellationToken.None);
            await _userService.DidNotReceiveWithAnyArgs()
                .ChangeEmail(default, default, default, default);
        }
    }
}
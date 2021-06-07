using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class When_ConfirmChangeEmail_called_for_existing_user_with_correct_password_and_expired_token : ConfirmChangeEmailBase
    {
        [SetUp]
        public void Setup()
        {
            _userManager
                .CheckPasswordAsync(default, default)
                .ReturnsForAnyArgs(true);

            _userManager
                .ChangeEmailAsync(default, default, default)
                .ReturnsForAnyArgs(IdentityResult.Failed(new IdentityErrorDescriber().InvalidToken()));
        }

        [Test]
        public async Task Then_returns_token_errors()
        {
            var result = await _sut.Handle(_request, CancellationToken.None);
            result.Should().BeEquivalentTo(new
            {
                PasswordError = (string)null,
                TokenError = true,
            });
        }
    }
}
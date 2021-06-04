using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class When_ConfirmChangeEmail_called_for_existing_user_incorrect_password : ConfirmChangeEmailBase
    {
        [SetUp]
        public void Setup()
        {
            _userManager
                .ChangeEmailAsync(default, default, default)
                .ReturnsForAnyArgs(IdentityResult.Failed(new IdentityErrorDescriber().PasswordMismatch()));
        }

        [Test]
        public async Task Then_returns_token_errors()
        {
            var result = await _sut.Handle(_request, CancellationToken.None);
            result.Should().BeEquivalentTo(new
            {
                PasswordError = "Incorrect password",
                TokenError = false,
            });
        }
    }
}
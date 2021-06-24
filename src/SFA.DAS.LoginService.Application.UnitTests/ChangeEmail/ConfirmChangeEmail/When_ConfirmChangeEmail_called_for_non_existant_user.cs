using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Data.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.ConfirmChangeEmail
{
    [TestFixture]
    public class When_ConfirmChangeEmail_called_for_non_existant_user : ConfirmChangeEmailBase
    {
        [SetUp]
        public void Setup()
        {
            _request.CurrentEmailAddress = "unknown@example.com";
            _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((LoginUser)null);
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

        [Test]
        public async Task Then_does_not_attempt_to_change_email()
        {
            try
            {
                await _sut.Handle(_request, CancellationToken.None);
            }
            catch
            { }
            await _userManager.DidNotReceiveWithAnyArgs()
                .ChangeEmailAsync(default, default, default);
        }
    }
}

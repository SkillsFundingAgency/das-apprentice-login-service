using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Data.Entities;
using System;
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
            _userService.FindByEmail(Arg.Any<string>()).Returns((LoginUser)null);
        }

        [Test]
        public void Then_returns_token_errors()
        {
            _sut.Invoking(x => x.Handle(_request, CancellationToken.None))
                .Should().Throw<ApplicationException>()
                .WithMessage("Current User's email `unknown@example.com` does not exist");
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
            await _userService.DidNotReceiveWithAnyArgs()
                .ChangeEmail(default, default, default, default);
        }
    }
}
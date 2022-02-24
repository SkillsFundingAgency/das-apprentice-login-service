using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.StartChangeEmail
{
    public class When_new_email_is_valid : CreateStartChangeEmailHandlerTestBase
    {
        [Test]
        public async Task And_user_is_not_found_Then_throw_exception()
        {
            var request = CreateStartChangeEmailRequest();
            request.SubjectId = Guid.NewGuid().ToString();
            try
            {
                await Sut.Handle(request, CancellationToken.None);
                Assert.Fail();
            }
            catch (ApplicationException e)
            {
                e.Message.Should().Contain($"{request.SubjectId} does not exist");
            }
        }

        [Test]
        public async Task Then_email_is_sent_with_correct_confirm_url()
        {
            var request = CreateStartChangeEmailRequest();
            var expectedLink =
                $"https://baseurl/profile/{ClientId}/changeemail/confirm?email={HttpUtility.UrlEncode(request.NewEmailAddress)}&token={HttpUtility.UrlEncode(Token)}";
            await Sut.Handle(request, CancellationToken.None);

            await EmailService.Received().SendChangeEmailCode(Arg.Is<ChangeUserEmailViewModel>(p => p.ConfirmEmailLink == expectedLink));
        }

        [Test]
        public async Task Then_email_is_sent_with_correct_name()
        {
            var request = CreateStartChangeEmailRequest();
            await Sut.Handle(request, CancellationToken.None);

            await EmailService.Received().SendChangeEmailCode(Arg.Is<ChangeUserEmailViewModel>(p => p.FamilyName == User.FamilyName && p.GivenName == User.GivenName));
        }

        [Test]
        public async Task Then_email_is_sent_to_new_email_address()
        {
            var request = CreateStartChangeEmailRequest();
            await Sut.Handle(request, CancellationToken.None);

            await EmailService.Received().SendChangeEmailCode(Arg.Is<ChangeUserEmailViewModel>(p => p.EmailAddress == request.NewEmailAddress));
        }
    }
}
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.StartChangeEmail
{
    public class When_new_email_is_valid_with_extra_whitespace : CreateStartChangeEmailHandlerTestBase
    {
        [Test]
        public async Task Then_email_is_sent_to_trimmed_new_email_address()
        {
            var request = CreateStartChangeEmailRequest();
            var trimmedEmail = request.NewEmailAddress;
            request.NewEmailAddress = $" {trimmedEmail} ";
            request.ConfirmEmailAddress = $"\t{trimmedEmail}\t";

            await Sut.Handle(request, CancellationToken.None);

            await EmailService.Received().SendChangeEmailCode(Arg.Is<ChangeUserEmailViewModel>(p => p.EmailAddress == trimmedEmail));
        }
    }
}
using System.Threading.Tasks;
using AutoFixture;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using SFA.DAS.Apprentice.LoginService.Messages;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "SendInvitation")]
    public class SendInvitationSteps
    {
        private readonly TestContext _context;
        private Fixture _f;
        private string _userEmail;
        private SendInvitationCommand _message;

        public SendInvitationSteps(TestContext context)
        {
            _context = context;
            _userEmail = "new-user@test,com";
            _f = new Fixture();
        }

        [When(@"the SendInvitationCommand is received")]
        public async Task WhenTheSendInvitationCommandIsReceived()
        {
            _message = _f.Build<SendInvitationCommand>()
                .With(x => x.Email, _userEmail)
                .With(x => x.UserRedirect, "https://test.com/")
                .With(x => x.Callback, "https://test.com/")
                .Create();

            await _context.WaitFor<MessageContext>(async () =>
                await _context.TestMessageBus.Send(_message));
        }

        [Then(@"the invitation is forwarded to the api correctly")]
        public void ThenTheInvitationIsForwardedToTheApiCorrectly()
        {
            //ScenarioContext.Current.Pending();
        }



        //[Then(@"the invitation is recorded")]
        //public void ThenTheInvitationIsRecorded()
        //{
        //    var invitation = _context.LoginContext.Invitations.SingleOrDefault(x => x.Email == _userEmail);
        //    invitation.Should().NotBeNull();
        //    invitation.Email.Should().Be(_userEmail);
        //    invitation.GivenName.Should().Be(_message.GivenName);
        //    invitation.FamilyName.Should().Be(_message.FamilyName);
        //    invitation.SourceId.Should().Be(_message.SourceId.ToString());
        //    invitation.ValidUntil.Should().BeAfter(DateTime.UtcNow);
        //    invitation.ClientId.Should().Be(_client.Id);

        //    invitation.CallbackUri.Should().NotBeNull();
        //    invitation.CallbackUri.AbsoluteUri.Should().Be(_message.Callback);

        //    invitation.UserRedirectUri.Should().NotBeNull();
        //    invitation.UserRedirectUri.AbsoluteUri.Should().Be(_message.UserRedirect);

        //}

        //[Then(@"the invitation is logged")]
        //public void ThenTheInvitationIsLogged()
        //{
        //    var log = _context.LoginContext.UserLogs.SingleOrDefault(x => x.Email == _userEmail);
        //    log.Should().NotBeNull();
        //    log.Action.Should().Be("Invite");
        //    log.Result.Should().Be("Invited");
        //    log.DateTime.Should().BeBefore(DateTime.UtcNow);
        //}


    }
}

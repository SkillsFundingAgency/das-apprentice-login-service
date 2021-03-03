using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using SFA.DAS.Apprentice.LoginService.Messages;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Data.JsonObjects;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "SendInvitation")]
    public class SendInvitationSteps
    {
        private readonly TestContext _context;
        private Fixture _f;
        private Client _client;
        private EmailTemplate _emailWhenUserExistsTemplate;
        private EmailTemplate _emailNewUserTemplate;
        private string _userEmail;
        private SendInvitationCommand _message;

        public SendInvitationSteps(TestContext context)
        {
            _context = context;
            _f = new Fixture();
            _emailWhenUserExistsTemplate = new EmailTemplate {Name = "LoginSignupError", TemplateId = Guid.NewGuid()};
            _emailNewUserTemplate = new EmailTemplate {Name = "SignUpInvitation", TemplateId = Guid.NewGuid()};
        }

        [Given(@"the login service has a client for this request")]
        public void GivenTheLoginServiceHasAClientForThisRequest()
        {
            var serviceDetails = _f.Create<ServiceDetails>();
            serviceDetails.EmailTemplates.Add(_emailWhenUserExistsTemplate);
            serviceDetails.EmailTemplates.Add(_emailNewUserTemplate);

            _client = new Client
            {
                AllowInvitationSignUp = true, 
                AllowLocalSignUp = false, 
                Id = Guid.NewGuid(),
                IdentityServerClientId = "Apprentice",
                ServiceDetails = serviceDetails
            };

            _context.LoginContext.Clients.Add(_client);
            _context.LoginContext.SaveChanges();
        }

        [Given(@"the apprentice's email does not already exist")]
        public void GivenTheApprenticeDoesNotAlreadyExist()
        {
            _userEmail = "new-user@test,com";
        }

        [When(@"the SendInvitationCommand is received")]
        public async Task WhenTheSendInvitationCommandIsReceived()
        {
            _message = _f.Build<SendInvitationCommand>()
                .With(x=>x.ClientId, _client.Id)
                .With(x => x.Email, _userEmail)
                .With(x => x.UserRedirect, "https://test.com/")
                .With(x => x.Callback, "https://test.com/")
                .Create();

            await _context.WaitFor<MessageContext>(async () =>
                await _context.TestMessageBus.Send(_message));
        }

        [Then(@"the invitation is recorded")]
        public void ThenTheInvitationIsRecorded()
        {
            var invitation = _context.LoginContext.Invitations.SingleOrDefault(x => x.Email == _userEmail);
            invitation.Should().NotBeNull();
            invitation.Email.Should().Be(_userEmail);
            invitation.GivenName.Should().Be(_message.GivenName);
            invitation.FamilyName.Should().Be(_message.FamilyName);
            invitation.SourceId.Should().Be(_message.SourceId.ToString());
            invitation.ValidUntil.Should().BeAfter(DateTime.UtcNow);
            invitation.ClientId.Should().Be(_client.Id);

            invitation.CallbackUri.Should().NotBeNull();
            invitation.CallbackUri.AbsoluteUri.Should().Be(_message.Callback);

            invitation.UserRedirectUri.Should().NotBeNull();
            invitation.UserRedirectUri.AbsoluteUri.Should().Be(_message.UserRedirect);

        }

        [Then(@"the invitation is logged")]
        public void ThenTheInvitationIsLogged()
        {
            var log = _context.LoginContext.UserLogs.SingleOrDefault(x => x.Email == _userEmail);
            log.Should().NotBeNull();
            log.Action.Should().Be("Invite");
            log.Result.Should().Be("Invited");
            log.DateTime.Should().BeBefore(DateTime.UtcNow);
        }

        [Then(@"the invitation email is sent")]
        public void ThenTheInvitationEmailIsSent()
        {
        }

    }
}

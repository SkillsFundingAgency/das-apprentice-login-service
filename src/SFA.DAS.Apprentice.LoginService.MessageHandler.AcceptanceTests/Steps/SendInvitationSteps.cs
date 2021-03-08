using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Apprentice.LoginService.Messages;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

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
        private Guid _clientId;
        private SendInvitationResponse _response;
        private WaitForResult _waitResult;

        public SendInvitationSteps(TestContext context)
        {
            _context = context;
            _userEmail = "new-user@test,com";
            _clientId = Guid.NewGuid();
            _fixture = new Fixture();
            _response = _f.Create<SendInvitationResponse>();
        }

        [Given(@"we have created a valid SendInvitationCommand")]
        public void GivenWeHaveCreatedAValidSendInvitationCommand()
        {
            _message = _f.Build<SendInvitationCommand>()
                .With(x=>x.ClientId, _clientId)
                .With(x => x.Email, _userEmail)
                .With(x => x.UserRedirect, "https://test.com/")
                .With(x => x.Callback, "https://test.com/")
                .Create();
        }

        [Given(@"the login service api is ready to respond")]
        public void GivenTheLoginServiceApiIsReadyToRespond()
        {
            _context.LoginServiceApi.MockServer
                .Given(
                    Request.Create().WithPath($"/Invitations/{_message.ClientId}")
                        .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode((int)HttpStatusCode.OK)
                    .WithBodyAsJson(_response));
        }

        [When(@"the SendInvitationCommand is received")]
        public async Task WhenTheSendInvitationCommandIsReceived()
        {
           _waitResult = await _context.WaitFor<MessageContext>(async () =>
                await _context.TestMessageBus.Send(_message));
        }

        [Then(@"the invitation is forwarded to the api correctly")]
        public void ThenTheInvitationIsForwardedToTheApiCorrectly()
        {
            var logs = _context.LoginServiceApi.MockServer.LogEntries;
            logs.Should().HaveCount(1);
            var request = JsonConvert.DeserializeObject<SendInvitationRequest>(logs.First().RequestMessage.Body);

            request.Email.Should().Be(_message.Email);
            request.SourceId.Should().Be(_message.SourceId);
            request.GivenName.Should().Be(_message.GivenName);
            request.FamilyName.Should().Be(_message.FamilyName);
            request.OrganisationName.Should().Be(_message.OrganisationName);
            request.ApprenticeshipName.Should().Be(_message.ApprenticeshipName);
            request.Callback.Should().Be(_message.Callback);
            request.UserRedirect.Should().Be(_message.UserRedirect);
        }
    }
}

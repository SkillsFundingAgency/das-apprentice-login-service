using AutoFixture.NUnit3;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Steps
{
    public class SendInvitationTest
    {
        [Test, DomainAutoData]
        public async Task Forward_SendInvitation_to_api(
            [Frozen] IInvitationApi api,
            SendInvitationCommandHandler sut,
            SendInvitation evt)
        {
            await sut.Handle(evt, new TestableMessageHandlerContext());

            await api.Received().SendInvitation(
                evt.ClientId,
                Arg.Is<SendInvitationRequest>(r =>
                r.Email == evt.Email
                && r.GivenName == evt.GivenName
                && r.FamilyName == evt.FamilyName
                && r.OrganisationName == evt.OrganisationName
                && r.ApprenticeshipName == evt.ApprenticeshipName
                && r.SourceId == evt.SourceId
                && r.Callback.ToString() == evt.Callback
                && r.UserRedirect.ToString() == evt.UserRedirect));
        }

        [Test, DomainAutoData]
        public async Task Replies_to_sender(
            [Frozen] IInvitationApi api,
            SendInvitationCommandHandler sut,
            SendInvitation evt,
            SendInvitationResponse response)
        {
            var context = new TestableMessageHandlerContext();
            api
                .SendInvitation(Arg.Any<Guid>(), Arg.Any<SendInvitationRequest>())
                .Returns(response);

            await sut.Handle(evt, context);

            Assert.That(context.RepliedMessages, Is.Not.Empty);
            Assert.That(context.RepliedMessages[0].Message, Is.InstanceOf<SendInvitationReply>());
            var reply = context.RepliedMessages[0].Message as SendInvitationReply;
            Assert.That(reply.Invited, Is.EqualTo(response.Invited));
            Assert.That(reply.Message, Is.EqualTo(response.Message));
            Assert.That(reply.InvitationId, Is.EqualTo(response.InvitationId));
            Assert.That(reply.ExistingUserId, Is.EqualTo(response.ExistingUserId));
            Assert.That(reply.LoginLink, Is.EqualTo(response.LoginLink));
        }
    }
}
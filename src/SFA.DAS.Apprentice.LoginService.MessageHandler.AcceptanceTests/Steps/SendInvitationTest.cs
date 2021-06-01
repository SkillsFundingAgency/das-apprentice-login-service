using AutoFixture.NUnit3;
using NServiceBus.Testing;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
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
    }
}
using MediatR;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using System.Threading.Tasks;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class SendInvitationCommandHandler : IHandleMessages<SendInvitation>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendInvitationCommandHandler> _log;

        public SendInvitationCommandHandler(IMediator mediator, ILogger<SendInvitationCommandHandler> log)
        {
            _mediator = mediator;
            _log = log;
        }

        public async Task Handle(SendInvitation message, IMessageHandlerContext context)
        {
            var response = await _mediator.Send(new CreateInvitationRequest
            {
                ClientId = message.ClientId,
                Email = message.Email,
                GivenName = message.GivenName,
                FamilyName = message.FamilyName,
                SourceId = message.SourceId,
                Callback = message.Callback,
                UserRedirect = message.UserRedirect,
                OrganisationName = message.OrganisationName,
                ApprenticeshipName = message.ApprenticeshipName,
            });

            _log.LogInformation(
                $"Completed {typeof(SendInvitation)} InvitationId : {response?.InvitationId} Invited : {response?.Invited}, Message : { response?.Message}");

            await context.Reply(new SendInvitationReply
            {
                ClientId = message.ClientId,
                Message = response.Message,
                Invited = response.Invited,
                InvitationId = response.InvitationId,
                ExistingUserId = response.ExistingUserId,
                LoginLink = response.LoginLink,
            });
        }
    }
}
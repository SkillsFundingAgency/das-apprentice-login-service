using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class SendInvitationCommandHandler : IHandleMessages<SendInvitation>
    {
        private readonly IInvitationApi _api;
        private readonly ILogger _log;

        public SendInvitationCommandHandler(IInvitationApi api, ILogger log)
        {
            _api = api;
            _log = log;
        }

        public async Task Handle(SendInvitation message, IMessageHandlerContext context)
        {
            var response = await _api.SendInvitation(message.ClientId, new SendInvitationRequest
            {
                Email = message.Email,
                GivenName = message.GivenName,
                FamilyName = message.FamilyName,
                SourceId = message.SourceId,
                Callback = new Uri(message.Callback),
                UserRedirect = new Uri(message.UserRedirect),
                OrganisationName = message.OrganisationName,
                ApprenticeshipName = message.ApprenticeshipName,
                Inviter = null
            });

            _log.LogInformation(
                $"Completed {typeof(SendInvitation)} InvitationId : {response?.InvitationId} Invited : {response?.Invited}, Message : { response?.Message}");

            await context.Reply(new SendInvitationReply
            {
                Message = response.Message,
                Invited = response.Invited,
                InvitationId = response.InvitationId,
                ExistingUserId = response.ExistingUserId,
                LoginLink = response.LoginLink,
            });
        }
    }
}
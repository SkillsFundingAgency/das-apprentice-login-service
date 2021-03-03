using System;
using System.Net.Http;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class SendInvitationCommandHandler
    {
        private readonly IMediator _mediator;

        public SendInvitationCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("HandleSendInvitationCommand")]
        public async Task RunCommand(
            [NServiceBusTrigger(Endpoint = QueueNames.SendInvitationCommand)] SendInvitationCommand sendInvitationCommand,
            ILogger log)
        {
            try
            {
                log.LogInformation(
                    $"Received {typeof(SendInvitationCommand)} SourceId : {sendInvitationCommand.SourceId} ClientId : {sendInvitationCommand.ClientId}");

                var response = await _mediator.Send(new CreateInvitationRequest
                {
                    Email = sendInvitationCommand.Email,
                    GivenName = sendInvitationCommand.GivenName,
                    FamilyName = sendInvitationCommand.FamilyName,
                    SourceId = sendInvitationCommand.SourceId.ToString(),
                    Callback = new Uri(sendInvitationCommand.Callback),
                    UserRedirect = new Uri(sendInvitationCommand.UserRedirect),
                    ClientId = sendInvitationCommand.ClientId,
                    IsInvitationToOrganisation = false,
                    Inviter = "Automatic",
                    ApprenticeshipName = sendInvitationCommand.ApprenticeshipName,
                    OrganisationName = sendInvitationCommand.OrganisationName
                });

                log.LogInformation(
                    $"Completed {typeof(SendInvitationCommand)} InvitationId : {response.InvitationId} Invited : {response.Invited}");
            }
            catch (Exception e)
            {
                log.LogError($"Errored when processing {typeof(SendInvitationCommand)}", e);
            }
        }

#if DEBUG
        [FunctionName("HandleSendInvitationCommandTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "POST", Route = "test-send-invitation-command")] HttpRequestMessage req,
            ILogger log)
        {
            log.LogInformation("Calling test-send-invitation-command");

            try
            {
                var command = JsonConvert.DeserializeObject<SendInvitationCommand>(await req.Content.ReadAsStringAsync());
                await RunCommand(command, log);
                return new AcceptedResult();
            }
            catch (Exception e)
            {
                log.LogError(e, "Error Calling test-send-invitation-command");
                return new BadRequestResult();
            }
        }
#endif
    }
}

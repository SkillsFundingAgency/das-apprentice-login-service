using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService;
using SFA.DAS.Apprentice.LoginService.Messages;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class SendInvitationCommandHandler
    {
        private readonly IInvitationApi _api;

        public SendInvitationCommandHandler(IInvitationApi api)
        {
            _api = api;
        }

        [FunctionName("HandleSendInvitationCommand")]
        public async Task RunCommand(
            [NServiceBusTrigger(Endpoint = QueueNames.SendInvitationCommand)] SendInvitationCommand sendInvitationCommand,
            ILogger log)
        {
            try
            {
                var response = await _api.SendInvitation(sendInvitationCommand.ClientId, new SendInvitationRequest
                {
                    Email = sendInvitationCommand.Email,
                    GivenName = sendInvitationCommand.GivenName,
                    FamilyName = sendInvitationCommand.FamilyName,
                    SourceId = sendInvitationCommand.SourceId,
                    Callback = new Uri(sendInvitationCommand.Callback),
                    UserRedirect = new Uri(sendInvitationCommand.UserRedirect),
                    OrganisationName = sendInvitationCommand.OrganisationName,
                    ApprenticeshipName = sendInvitationCommand.ApprenticeshipName,
                    Inviter = null
                });

                log.LogInformation(
                    $"Completed {typeof(SendInvitationCommand)} InvitationId : {response?.InvitationId} Invited : {response?.Invited}, Message : { response?.Message}");
            }
            catch (Exception e)
            {
                log.LogError($"Errored when processing {typeof(SendInvitationCommand)}", e);
                throw;
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

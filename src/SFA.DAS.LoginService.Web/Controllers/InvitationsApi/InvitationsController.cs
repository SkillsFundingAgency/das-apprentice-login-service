using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Web.Controllers.InvitationsApi.ViewModels;

namespace SFA.DAS.LoginService.Web.Controllers.InvitationsApi
{
    public class InvitationsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InvitationsController> _logger;

        public InvitationsController(IMediator mediator, ILogger<InvitationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("/Invitations/{clientId}")]
        public async Task<ActionResult<CreateInvitationResponse>> Invite(Guid clientId, [FromBody] InvitationRequestViewModel createInvitationRequest)
        {
            try
            {
               var response = await _mediator.Send(new CreateInvitationRequest()
               {
                   Email = createInvitationRequest.Email,
                   GivenName = createInvitationRequest.GivenName ?? "Unknown",
                   FamilyName = createInvitationRequest.FamilyName ?? "Unknown",
                   OrganisationName = createInvitationRequest.OrganisationName ?? "Unknown",
                   ApprenticeshipName = createInvitationRequest.ApprenticeshipName ?? "Unknown",
                   SourceId = createInvitationRequest.SourceId,
                   Callback = createInvitationRequest.Callback,
                   UserRedirect = createInvitationRequest.UserRedirect,
                   ClientId = clientId,
                   IsInvitationToOrganisation = false,
               });
               _logger.LogDebug($"Received Response from CreateInvitationHandler: Invited: {response.Invited} Message: {response.Message}");
               return response;
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        [HttpPost("/Invitations/{clientId}/inviteToOrganisation")]
        public async Task<ActionResult<CreateInvitationResponse>> InviteToOrganisation(Guid clientId, [FromBody] InvitationRequestViewModel createInvitationRequest)
        {
            try
            {
                var response = await _mediator.Send(new CreateInvitationRequest()
                {
                    Email = createInvitationRequest.Email,
                    GivenName = createInvitationRequest.GivenName,
                    FamilyName = createInvitationRequest.FamilyName,
                    SourceId = createInvitationRequest.SourceId,
                    Callback = createInvitationRequest.Callback,
                    UserRedirect = createInvitationRequest.UserRedirect,
                    ClientId = clientId,
                    IsInvitationToOrganisation = true,
                    Inviter = createInvitationRequest.Inviter,
                    OrganisationName = createInvitationRequest.OrganisationName
                });
                _logger.LogDebug($"Received Response from CreateInvitationHandler: Invited: {response.Invited} Message: {response.Message}");
                return response;
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
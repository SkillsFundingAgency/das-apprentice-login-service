using MediatR;
using System;

namespace SFA.DAS.LoginService.Application.Invitations.CreateInvitation
{
    public class CreateInvitationRequest : IRequest<CreateInvitationResponse>
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string SourceId { get; set; }
        public Uri UserRedirect { get; set; }
        public Uri Callback { get; set; }
        public Guid ClientId { get; set; }
        public bool IsInvitationToOrganisation { get; set; }
        public string Inviter { get; set; }
    }
}
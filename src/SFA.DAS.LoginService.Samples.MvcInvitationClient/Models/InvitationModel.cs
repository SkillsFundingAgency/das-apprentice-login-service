using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;

namespace SFA.DAS.LoginService.Samples.MvcInvitationClient.Models
{
    public class InvitationModel
    {
        public string Email { get; set; }
        public CreateInvitationResponse InvitationResponse{ get; set; }
    }
}
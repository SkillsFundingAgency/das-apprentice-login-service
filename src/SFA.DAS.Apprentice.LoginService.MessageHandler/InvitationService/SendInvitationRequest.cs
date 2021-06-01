using System;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService
{
    public class SendInvitationRequest
    {
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string SourceId { get; set; }
        public Uri Callback { get; set; }
        public Uri UserRedirect { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string Inviter { get; set; }
    }
}
using System;

namespace SFA.DAS.Apprentice.LoginService.Messages
{
    public class SendInvitationCommand
    {
        public Guid ClientId { get; set; }
        public string SourceId { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string Callback { get; set; }
        public string UserRedirect { get; set; }
    }
}

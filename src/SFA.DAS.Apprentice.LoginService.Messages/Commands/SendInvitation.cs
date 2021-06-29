using System;

namespace SFA.DAS.Apprentice.LoginService.Messages
{
    public class SendInvitation
    {
        public Guid ClientId { get; set; }
        public string SourceId { get; set; }
        public string Email { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public Uri Callback { get; set; }
        public Uri UserRedirect { get; set; }
    }

    public class SendInvitationReply
    {
        public string Message { get; set; }
        public bool Invited { get; set; }
        public Guid InvitationId { get; set; }
        public string ExistingUserId { get; set; }
        public Guid ClientId { get; set; }
        public string ServiceName { get; set; }
        public string LoginLink { get; set; }
    }
}

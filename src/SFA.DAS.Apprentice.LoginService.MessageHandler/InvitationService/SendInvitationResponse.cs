using System;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService
{
    public class SendInvitationResponse
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
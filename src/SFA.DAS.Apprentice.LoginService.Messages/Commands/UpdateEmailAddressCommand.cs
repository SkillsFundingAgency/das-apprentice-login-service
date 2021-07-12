using System;

namespace SFA.DAS.Apprentice.LoginService.Messages.Commands
{
    public class UpdateEmailAddressCommand
    {
        public Guid ApprenticeId { get; set; }
        public string CurrentEmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
    }
}

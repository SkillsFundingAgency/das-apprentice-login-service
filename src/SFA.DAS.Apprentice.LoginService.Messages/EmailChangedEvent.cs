using System;

namespace SFA.DAS.Apprentice.LoginService.Messages
{
    public class EmailChangedEvent
    {
        public Guid ApprenticeId { get; set; }
        public string CurrentEmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
    }
}

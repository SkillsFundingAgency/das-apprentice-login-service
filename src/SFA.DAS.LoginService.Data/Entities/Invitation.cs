using System;

namespace SFA.DAS.LoginService.Data.Entities
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string SourceId { get; set; }
        public DateTime ValidUntil { get; set; }
        public Uri CallbackUri { get; set; }
        public Uri UserRedirectUri { get; set; }
        public bool IsUserCreated { get; set; }
        public bool IsCalledBack { get; set; }
        public DateTime? CallbackDate { get; set; }
        public Guid ClientId { get; set; }
    }
}
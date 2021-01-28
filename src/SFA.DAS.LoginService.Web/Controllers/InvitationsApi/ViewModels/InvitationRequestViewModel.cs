using System;

namespace SFA.DAS.LoginService.Web.Controllers.InvitationsApi.ViewModels
{
    public class InvitationRequestViewModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string SourceId { get; set; }
        public Uri Callback { get; set; }
        public Uri UserRedirect { get; set; }
        public string Inviter { get; set; }
    }
}
namespace SFA.DAS.LoginService.Application.Services.EmailServiceViewModels
{
    public class InvitationEmailViewModel : EmailViewModel
    {
        public override string EmailAddress { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Contact { get; set; }
        public string OrganisationName { get; set; }
        public string ApprenticeshipName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceTeam { get; set; }
        public string LoginLink { get; set; }
        public string Inviter { get; set; }
        public string CreateAccountLink { get; set; }
    }
}
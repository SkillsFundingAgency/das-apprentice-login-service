namespace SFA.DAS.LoginService.Application.Services.EmailServiceViewModels
{
    public class ChangeUserEmailViewModel : EmailViewModel
    {
        public string ConfirmEmailLink { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
    }
}
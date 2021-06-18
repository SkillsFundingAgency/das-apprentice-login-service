namespace SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail
{
    public class StartChangeEmailResponse
    {
        public string NewEmailAddressError { get; set; }
        public string ConfirmEmailAddressError { get; set; }

        public bool HasErrors => !string.IsNullOrEmpty(NewEmailAddressError) ||
                                 !string.IsNullOrEmpty(ConfirmEmailAddressError);
    }
}
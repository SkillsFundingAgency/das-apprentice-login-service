namespace SFA.DAS.LoginService.Application.CreateAccount
{
    public class CreateAccountResponse
    {
        public bool DuplicateEmail { get; set; }
        public bool PasswordValid { get; set; }
        public string EmailAddressError { get; set; }
        public string ConfirmEmailAddressError { get; set; }

        public bool HasErrors => !string.IsNullOrEmpty(EmailAddressError) ||
                                 !string.IsNullOrEmpty(ConfirmEmailAddressError);
    }
}
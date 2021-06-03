namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class ConfirmChangeEmailResponse
    {
        public string PasswordError { get; set; }
        public bool TokenError { get; set; }

        public bool HasErrors
            => !string.IsNullOrEmpty(PasswordError)
            || TokenError;
    }
}
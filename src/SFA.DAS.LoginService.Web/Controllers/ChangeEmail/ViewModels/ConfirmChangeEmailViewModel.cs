using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels
{
    public class ConfirmChangeEmailViewModel
    {
        public string Token { get; set; }
        public string NewEmailAddress { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        public string Password { get; set; }

        public string TempCurrentEmail { get; set; }
    }
}
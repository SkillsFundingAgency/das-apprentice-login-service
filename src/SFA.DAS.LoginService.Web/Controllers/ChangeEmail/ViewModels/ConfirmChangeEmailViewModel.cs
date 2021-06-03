using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels
{
    public class ConfirmChangeEmailViewModel
    {
        public string NewEmailAddress { get; set; }

        public string Token { get; set; }

        public bool TokenInvalid { get; set; }

        [Required(ErrorMessage = "Enter your password")]
        public string Password { get; set; }

        public string TempCurrentEmail { get; set; }
        public Guid ClientId { get; internal set; }
    }
}
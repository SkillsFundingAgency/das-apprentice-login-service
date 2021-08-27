using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels
{
    public class CreateAccountViewModel
    {
        public Guid ClientId { get; set; }
        public string ReturnUrl { get; set; }
        public string Backlink { get; set; }

        [Required(ErrorMessage = "Enter a password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm your password")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Enter an email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
    }
}

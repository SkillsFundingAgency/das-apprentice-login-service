using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels
{
    public class CreateAccountViewModel
    {
        public Guid ClientId { get; set; }
        public string ReturnUrl { get; set; }
        public string Backlink { get; set; }

        [Required(ErrorMessage = "Enter an email address")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Re-enter the email address")]
        [EmailAddress(ErrorMessage = "Re-enter a valid email address")]
        [Compare(nameof(Email), ErrorMessage = "Enter the same email address")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Enter a password")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Re-enter the password")]
        [Compare(nameof(Password), ErrorMessage = "Enter the same password")]
        public string ConfirmPassword { get; set; }
    }
}

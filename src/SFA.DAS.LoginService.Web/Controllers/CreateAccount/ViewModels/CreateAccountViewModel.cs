using System;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels
{
    public class CreateAccountViewModel
    {
        public Guid ClientId { get; set; }
        public string ReturnUrl { get; set; }
        public string Backlink { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please re-enter the email address")]
        [Compare(nameof(Email), ErrorMessage = "Please enter the same email address")]
        public string ConfirmEmail { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Plesae re-enter the password")]
        [Compare(nameof(Password), ErrorMessage = "Please enter the same password")]
        public string ConfirmPassword { get; set; }
    }
}

using IdentityServer4.Models;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.InvitationsWeb.ViewModels
{
    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Enter a password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm your password")]
        public string ConfirmPassword { get; set; }
        public string NewEmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }
    }
}

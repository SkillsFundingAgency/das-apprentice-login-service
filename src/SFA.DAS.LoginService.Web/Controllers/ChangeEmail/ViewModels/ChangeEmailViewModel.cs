using System.ComponentModel.DataAnnotations;
using System.Net;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels
{
    public class ChangeEmailViewModel
    {
        [Required(ErrorMessage = "Enter the new email address")]
        public string NewEmailAddress { get; set; }
        [Required(ErrorMessage = "Re-type the new email address")]
        public string ConfirmEmailAddress { get; set; }
    }
}
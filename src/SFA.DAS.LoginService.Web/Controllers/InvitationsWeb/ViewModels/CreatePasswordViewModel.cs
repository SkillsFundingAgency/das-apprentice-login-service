using SFA.DAS.LoginService.Web.Controllers.ResetPassword.ViewModels;
using System;

namespace SFA.DAS.LoginService.Web.Controllers.InvitationsWeb.ViewModels
{
    public class CreatePasswordViewModel : PasswordViewModel
    {
        public Guid InvitationId { get; set; }
        public override bool ShowPrivacyTerms => true;
    }
}
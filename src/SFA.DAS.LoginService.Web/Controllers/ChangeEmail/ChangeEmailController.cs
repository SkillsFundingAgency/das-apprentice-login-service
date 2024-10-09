using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;
using SFA.DAS.LoginService.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LoginService.Web.AppStart;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    public class ChangeEmailController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public ChangeEmailController(IMediator mediator, HomePageRedirect homePageRedirect)
            : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [Authorize]
        [HttpGet("profile/{clientId}/changeemail")]
        public async Task<IActionResult> ChangeEmail(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [Authorize]
        [HttpPost("profile/{clientId}/changeemail")]
        public async Task<IActionResult> ChangeEmail([FromRoute] Guid clientId, [FromForm] ChangeEmailViewModel model)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [Authorize]
        [HttpGet("profile/{clientId}/waittoconfirmnewemail")]
        public IActionResult WaitToConfirmNewEmail([FromRoute] Guid clientId, [FromQuery] string email, [FromQuery] bool resend)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        private void SetModelState(StartChangeEmailResponse response)
        {
            if (response.NewEmailAddressError != null)
            {
                ViewData.ModelState.AddModelError("NewEmailAddress", response.NewEmailAddressError);
            }

            if (response.ConfirmEmailAddressError != null)
            {
                ViewData.ModelState.AddModelError("ConfirmEmailAddress", response.ConfirmEmailAddressError);
            }
        }
    }
}
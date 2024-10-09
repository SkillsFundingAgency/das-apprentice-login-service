using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Polly;
using SFA.DAS.LoginService.Application.ChangeEmail.ConfirmChangeEmail;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;
using SFA.DAS.LoginService.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.LoginService.Web.AppStart;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    public class ConfirmChangeEmailController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public ConfirmChangeEmailController(IMediator mediator, HomePageRedirect homePageRedirect) : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [HttpGet("profile/{clientId}/changeemail/confirm")]
        public IActionResult ConfirmChangeEmail(Guid clientId, [FromQuery] string email, [FromQuery] string token)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [HttpPost("profile/{clientId}/changeemail/confirm")]
        public async Task<IActionResult> ConfirmChangeEmail(Guid clientId, [FromForm] ConfirmChangeEmailViewModel model)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [Authorize]
        [HttpGet("profile/{clientId}/changeemail/changeemailsuccessful")]
        public async Task<IActionResult> ChangeEmailSuccessful(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        private void SetModelState(ConfirmChangeEmailResponse response, ConfirmChangeEmailViewModel model)
        {
            model.TokenInvalid = response.TokenError;

            if (response.PasswordError != null)
                ViewData.ModelState.AddModelError("Password", response.PasswordError);
        }
    }
}
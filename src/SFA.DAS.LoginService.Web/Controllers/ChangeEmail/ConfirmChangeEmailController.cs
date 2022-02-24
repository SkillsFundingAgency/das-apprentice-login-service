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

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    public class ConfirmChangeEmailController : BaseController
    {
        public ConfirmChangeEmailController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet("profile/{clientId}/changeemail/confirm")]
        public IActionResult ConfirmChangeEmail(Guid clientId, [FromQuery] string email, [FromQuery] string token)
        {
            return View(new ConfirmChangeEmailViewModel
            {
                ClientId = clientId,
                NewEmailAddress = email,
                Token = token,
            });
        }

        [HttpPost("profile/{clientId}/changeemail/confirm")]
        public async Task<IActionResult> ConfirmChangeEmail(Guid clientId, [FromForm] ConfirmChangeEmailViewModel model)
        {
            model.ClientId = clientId;
            var currentEmail = User.Identity.Name;

            var response = await Mediator.Send(new ConfirmChangeEmailRequest
            {
                SubjectId = User.Claims.Subject(),
                CurrentEmailAddress = currentEmail,
                NewEmailAddress = model.NewEmailAddress,
                Token = model.Token.Replace(" ", "+"),
                Password = model.Password,
            });

            if (response.HasErrors)
            {
                SetModelState(response, model);
                return View(model);
}

            var authenticated = await HttpContext.AuthenticateAsync();

            if (authenticated.Succeeded)
            {
                await HttpContext.SignInAsync(authenticated.Principal, authenticated.Properties);
            }

            return RedirectToAction("ChangeEmailSuccessful", new { ClientId = clientId });
        }

        [Authorize]
        [HttpGet("profile/{clientId}/changeemail/changeemailsuccessful")]
        public async Task<IActionResult> ChangeEmailSuccessful(Guid clientId)
        {
            SetViewBagClientId(clientId);

            var client = await Mediator.Send(new GetClientByIdRequest() { ClientId = clientId });

            return View(new ChangeEmailSuccessfulViewModel() { ReturnUrl = client.ServiceDetails.PostPasswordResetReturnUrl, ServiceName = client.ServiceDetails.ServiceName });
        }

        private void SetModelState(ConfirmChangeEmailResponse response, ConfirmChangeEmailViewModel model)
        {
            model.TokenInvalid = response.TokenError;

            if (response.PasswordError != null)
                ViewData.ModelState.AddModelError("Password", response.PasswordError);
        }
    }
}
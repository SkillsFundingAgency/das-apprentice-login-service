using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.ChangeEmail.ConfirmChangeEmail;
using SFA.DAS.LoginService.Application.ChangeEmail.ChangeEmailSuccessful;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    //[Route("profile/{clientId}/changeemail/confirm")]
    public class ConfirmChangeEmailController : BaseController
    {
        public ConfirmChangeEmailController(IMediator mediator) : base(mediator)
        {
        }

        [Authorize]
        [HttpGet]
        public IActionResult ConfirmChangeEmail(Guid clientId, [FromQuery] string email, [FromQuery] string token)
        {
            return View(new ConfirmChangeEmailViewModel
            {
                ClientId = clientId,
                NewEmailAddress = email,
                Token = token,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmChangeEmail(Guid clientId, [FromForm] ConfirmChangeEmailViewModel model)
        {
            var currentEmail = User.Identity.Name;

            var response = await Mediator.Send(new ConfirmChangeEmailRequest
            {
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

            //return Ok();
            return RedirectToAction("ChangeEmailSuccessful", new { ClientId = clientId });
        }

        //[Authorize]
        [HttpGet("profile/{clientId}/changeemail/changeemailsuccessful")]
        public async Task<IActionResult> ChangeEmailSuccessful(Guid clientId)
        {
            var client = await Mediator.Send(new ChangeEmailSuccessfulRequest { ClientId = clientId });
            var baseUrl = client.ServiceDetails.SupportUrl;
            var model = new ChangeEmailSuccessfulViewModel { ReturnUrl = baseUrl + "apprenticeships" };

            return View();
        }

        private void SetModelState(ConfirmChangeEmailResponse response, ConfirmChangeEmailViewModel model)
        {
            model.TokenInvalid = response.TokenError;

            if (response.PasswordError != null)
                ViewData.ModelState.AddModelError("Password", response.PasswordError);
        }
    }
}
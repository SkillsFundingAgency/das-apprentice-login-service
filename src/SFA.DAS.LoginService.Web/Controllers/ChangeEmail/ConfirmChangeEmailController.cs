using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.StartChangeEmail;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    [Route("profile/{clientId}/changeemail/confirm")]
    public class ConfirmChangeEmailController : BaseController
    {
        public ConfirmChangeEmailController(IMediator mediator) : base(mediator)
        {
        }

        //[Authorize]
        [HttpGet]
        public IActionResult ConfirmChangeEmail(Guid clientId, [FromQuery] string email, [FromQuery] string token)
        {
            return View(new ConfirmChangeEmailViewModel
            {
                NewEmailAddress = email,
                Token = token,
            });
        }

        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmChangeEmail(Guid clientId, [FromForm] ConfirmChangeEmailViewModel model)
        {
            var currentEmail = model.TempCurrentEmail;

            var response = await Mediator.Send(new ConfirmChangeEmailRequest
            {
                CurrentEmailAddress = currentEmail,
                NewEmailAddress = model.NewEmailAddress,
                Token = model.Token.Replace(" ", "+"),
                Password = model.Password,
            });

            if (response.HasErrors)
            {
                SetModelState(response);
                return View(model);
            }

            return Ok();
        }

        private void SetModelState(ConfirmChangeEmailResponse response)
        {
            if (response.PasswordError != null)
                ViewData.ModelState.AddModelError("Password", response.PasswordError);
            if (response.TokenError)
                ViewData.ModelState.AddModelError("Token", "Invalid token");
        }
    }
}
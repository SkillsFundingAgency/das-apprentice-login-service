using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    public class ChangeEmailController : BaseController
    {
        public ChangeEmailController(IMediator mediator)
            : base(mediator)
        {
        }

        [Authorize]
        [HttpGet("profile/{clientId}/changeemail")]
        public IActionResult ChangeEmail(Guid clientId)
        {
            var model = new ChangeEmailViewModel();

            return View(model);
        }

        [Authorize]
        [HttpPost("profile/{clientId}/changeemail")]
        public async Task<IActionResult> ChangeEmail([FromRoute] Guid clientId, [FromForm] ChangeEmailViewModel model)
        {
            var email = User.Identity.Name;

            var response = await Mediator.Send(new StartChangeEmailRequest
            {
                ClientId = clientId,
                CurrentEmailAddress = email,
                NewEmailAddress = model.NewEmailAddress,
                ConfirmEmailAddress = model.ConfirmEmailAddress
            });

            if (response.HasErrors)
            {
                SetModelState(response);
                return View("ChangeEmail", model);
            }

            return RedirectToAction("WaitToConfirmNewEmail", new
            {
                ClientId = clientId,
                Email = model.NewEmailAddress,
                Resend = model.Resend,
            });
        }

        [Authorize]
        [HttpGet("profile/{clientId}/waittoconfirmnewemail")]
        public IActionResult WaitToConfirmNewEmail([FromRoute] Guid clientId, [FromQuery] string email, [FromQuery] bool resend)
        {
            var model = new ChangeEmailViewModel
            {
                Backlink = $"/profile/{clientId}/changeemail",
                NewEmailAddress = email,
                ConfirmEmailAddress = email,
                Resend = resend,
            };

            return View(model);
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
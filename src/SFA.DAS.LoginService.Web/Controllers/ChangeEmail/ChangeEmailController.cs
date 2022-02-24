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
        public async Task<IActionResult> ChangeEmail(Guid clientId)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });

            var model = new ChangeEmailViewModel
            {
                Backlink = client.ServiceDetails.PostPasswordResetReturnUrl,
            };

            return View(model);
        }

        [Authorize]
        [HttpPost("profile/{clientId}/changeemail")]
        public async Task<IActionResult> ChangeEmail([FromRoute] Guid clientId, [FromForm] ChangeEmailViewModel model)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });
            model.Backlink = client.ServiceDetails.PostPasswordResetReturnUrl;

            var response = await Mediator.Send(new StartChangeEmailRequest
            {
                ClientId = clientId,
                SubjectId = User.Claims.Subject(),
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
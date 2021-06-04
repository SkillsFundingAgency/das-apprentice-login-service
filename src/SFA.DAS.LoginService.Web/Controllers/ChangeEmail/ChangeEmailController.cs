using MediatR;
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

        //[Authorize]
        [HttpGet("profile/{clientId}/changeemail")]
        public IActionResult ChangeEmail(Guid clientId)
        {
            var model = new ChangeEmailViewModel();

            return View(model);
        }

        //[Authorize]
        [HttpPost("profile/{clientId}/changeemail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail([FromRoute] Guid clientId, [FromForm] ChangeEmailViewModel model)
        {
            //var user = User.Identity;
            var email = "paul.graham@coprime.co.uk";  // user.Name;

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

            TempData["EmailChangeRequested"] = true;
            TempData["EmailChangeNewEmail"] = model.NewEmailAddress;
            return RedirectToAction("WaitForConfirmNewEmail", new { ClientId = clientId });
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

        [HttpGet("account/{clientId}/waitforconfirmnewemail")]
        public IActionResult WaitForConfirmNewEmail(Guid clientId)
        {
            return View();
        }
    }
}
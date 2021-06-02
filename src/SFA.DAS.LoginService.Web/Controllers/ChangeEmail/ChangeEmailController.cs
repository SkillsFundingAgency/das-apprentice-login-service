using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.StartChangeEmail;
using SFA.DAS.LoginService.Web.Controllers.ChangeEmail.ViewModels;

namespace SFA.DAS.LoginService.Web.Controllers.ChangeEmail
{
    public class ChangeEmailController : BaseController
    {
        public ChangeEmailController(IMediator mediator)
            : base(mediator)
        {
        }

        //[Authorize]
        //[HttpGet("profile/{clientId}/changeemail")]
        [HttpGet("profile/change-email")]
        public IActionResult ChangeEmail(Guid clientId)
        {
            var model = new ChangeEmailViewModel();

            return View(model);
        }

        //[Authorize]
        [HttpPost("profile/change-email")]
        [ValidateAntiForgeryToken]
        //[HttpPost("profile/{clientId}/changeemail")]
        //public async Task<IActionResult> ChangeEmail([FromRoute]Guid clientId, [FromForm]ChangeEmailViewModel model)
        public async Task<IActionResult> ChangeEmail([FromForm]ChangeEmailViewModel model)
        {

            //var user = User.Identity;
            var email = "paul.graham@coprime.co.uk";  // user.Name;

            var response = await Mediator.Send(new StartChangeEmailRequest
            {
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
            return RedirectToAction("ConfirmChangeEmail");
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

        [HttpGet("account/ConfirmChangeEmail")]
        public IActionResult ConfirmChangeEmail()
        {
            return View();
        }

    }
}
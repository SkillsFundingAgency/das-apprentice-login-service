using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.ResetPassword
{
    public class RequestPasswordResetController : BaseController
    {
        public RequestPasswordResetController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet("/ForgottenPassword/{clientId}")]
        public async Task<IActionResult> Get(Guid clientId)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });
            SetViewBagClientId(clientId);

            var vm = new RequestPasswordResetViewModel
            {
                ClientId = clientId,
                Backlink = client.ServiceDetails.SupportUrl,
            };

            return View("RequestPasswordReset", vm);
        }

        [HttpPost("/ForgottenPassword/{clientId}")]
        public async Task<IActionResult> Post(Guid clientId, RequestPasswordResetViewModel requestPasswordResetViewModel)
        {
            SetViewBagClientId(clientId);

            if (!ModelState.IsValid)
            {
                return View("RequestPasswordReset", requestPasswordResetViewModel);
            }

            await Mediator.Send(new RequestPasswordResetRequest { ClientId = clientId, Email = requestPasswordResetViewModel.Email });
            return RedirectToAction("CodeSent", new { clientId, email = requestPasswordResetViewModel.Email });
        }

        [HttpGet("/CodeSent")]
        public IActionResult CodeSent(Guid clientId, string email)
        {
            SetViewBagClientId(clientId);

            return View("CodeSent", new CodeSentViewModel() { Email = email });
        }
    }
}
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount
{
    public class CreateAccountController : BaseController
    {
        public CreateAccountController(IMediator mediator)
            : base(mediator)
        {
        }

        //[HttpGet("/CreateAccount/{clientId}/{requestId}")]

        [HttpGet("/CreateAccount/{clientId}")]
        public async Task<IActionResult> Index(Guid clientId)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });
            SetViewBagClientId(clientId);

            var vm = new CreateAccountViewModel
            {
                ClientId = clientId,
                Backlink = client.ServiceDetails.SupportUrl,
            };

            return View("CreateAccount", vm);


            //return View("CreateAccount");
        }

        //[HttpPost("/ForgottenPassword/{clientId}")]
        //public async Task<IActionResult> Post(Guid clientId, RequestPasswordResetViewModel requestPasswordResetViewModel)
        //{
        //    var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });
        //    requestPasswordResetViewModel.Backlink = client.ServiceDetails.SupportUrl;

        //    SetViewBagClientId(clientId);

        //    if (!ModelState.IsValid)
        //    {
        //        return View("RequestPasswordReset", requestPasswordResetViewModel);
        //    }

        //    await Mediator.Send(new RequestPasswordResetRequest { ClientId = clientId, Email = requestPasswordResetViewModel.Email });
        //    return RedirectToAction("CodeSent", new { clientId, email = requestPasswordResetViewModel.Email });
    }
}

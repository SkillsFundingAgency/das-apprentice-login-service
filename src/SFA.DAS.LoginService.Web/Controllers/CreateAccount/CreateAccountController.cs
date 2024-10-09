using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.CreateAccount;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels;
using System;
using System.Threading.Tasks;
using SFA.DAS.LoginService.Web.AppStart;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount
{
    public class CreateAccountController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public CreateAccountController(IMediator mediator, HomePageRedirect homePageRedirect)
            : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [HttpGet("/CreateAccount/{clientId}")]
        public async Task<IActionResult> Get(Guid clientId, string returnUrl)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
            
        }

        [HttpPost("/CreateAccount/{clientId}")]
        public async Task<ActionResult> Post(Guid clientId, CreateAccountViewModel vm)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        private void SetModelState(CreateAccountResponse response)
        {
            if (response.EmailAddressError != null)
            {
                ViewData.ModelState.AddModelError("EmailAddress", response.EmailAddressError);
            }

            if (response.ConfirmEmailAddressError != null)
            {
                ViewData.ModelState.AddModelError("ConfirmEmailAddress", response.ConfirmEmailAddressError);
            }
        }
    }
}
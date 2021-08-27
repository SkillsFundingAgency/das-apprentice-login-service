using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.CreateAccount;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.CreateAccount.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Controllers.CreateAccount
{
    public class CreateAccountController : BaseController
    {
        public CreateAccountController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet("/CreateAccount/{clientId}")]
        public async Task<IActionResult> Get(Guid clientId, string returnUrl)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });
            SetViewBagClientId(clientId);

            var vm = new CreateAccountViewModel
            {
                ClientId = clientId,
                Backlink = client.ServiceDetails.SupportUrl,
                ReturnUrl = returnUrl,
            };

            return View("CreateAccount", vm);
        }

        [HttpPost("/CreateAccount/{clientId}")]
        public async Task<ActionResult> Post(Guid clientId, CreateAccountViewModel vm)
        {
            var client = await Mediator.Send(new GetClientByIdRequest { ClientId = clientId });

            if (!ModelState.IsValid)
            {
                return View("CreateAccount", vm);
            }

            if (vm.Password == vm.ConfirmPassword)
            {
                var response = await Mediator.Send(new CreateAccountRequest { Email = vm.Email, Password = vm.Password });
                if (response.PasswordValid)
                {
                    return Redirect(vm.ReturnUrl);
                }

                ModelState.AddModelError("Password", "Password does not meet minimum complexity requirements");

                return View("CreateAccount",
                    new CreateAccountViewModel()
                    {
                        Password = vm.Password,
                        ConfirmPassword = vm.ConfirmPassword,
                        Email = vm.Email,
                        ConfirmEmail = vm.ConfirmEmail
                    });

                if (response.HasErrors)
                {
                    SetModelState(response);
                    return View("CreateAccount", vm);
                }
            }

            ModelState.AddModelError("Password", "Passwords should match");

            return View("CreateAccount", new CreateAccountViewModel()
            {
                Password = vm.Password,
                ConfirmPassword = vm.ConfirmPassword
            });
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
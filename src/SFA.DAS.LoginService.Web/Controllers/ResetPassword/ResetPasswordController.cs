using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.AppStart;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword.ViewModels;

namespace SFA.DAS.LoginService.Web.Controllers.ResetPassword
{
    public class ResetPasswordController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public ResetPasswordController(IMediator mediator, HomePageRedirect homePageRedirect)
            : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [HttpGet("/NewPassword/{clientId}/{requestId}")]
        public async Task<IActionResult> Get(Guid clientId, Guid requestId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [HttpPost("/NewPassword/{clientId}/{requestId}")]
        public async Task<IActionResult> Post(Guid clientId, Guid requestId, ResetPasswordViewModel viewModel)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [HttpGet("/PasswordResetSuccessful")]
        public async Task<IActionResult> PasswordResetSuccessful(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }
    }
}
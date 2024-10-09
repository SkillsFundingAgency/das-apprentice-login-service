using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.BuildLoginViewModel;
using SFA.DAS.LoginService.Application.ProcessLogin;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Web.AppStart;

namespace SFA.DAS.LoginService.Web.Controllers.Login
{
    public class LoginController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public LoginController(IMediator mediator, HomePageRedirect homePageRedirect)
             : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [HttpGet("/Account/Login")]
        public async Task<IActionResult> GetLogin(string returnUrl)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }

        [HttpPost("/Account/Login")]
        public async Task<IActionResult> PostLogin(LoginViewModel loginViewModel)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
            
        }
    }
}
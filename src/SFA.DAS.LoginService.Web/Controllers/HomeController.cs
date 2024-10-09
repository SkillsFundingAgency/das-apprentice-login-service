using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Web.AppStart;

namespace SFA.DAS.LoginService.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly HomePageRedirect _homePageRedirect;

        public HomeController(IMediator mediator, HomePageRedirect homePageRedirect)
            : base(mediator)
        {
            _homePageRedirect = homePageRedirect;
        }

        [HttpGet("/TermsOfUse")]
        public IActionResult TermsOfUse(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }
        [HttpGet("/Privacy")]
        public IActionResult Privacy(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }
        [HttpGet("/Cookies")]
        public IActionResult Cookies(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }
        [HttpGet("/CookieDetails")]
        public IActionResult CookieDetails(Guid clientId)
        {
            return RedirectPermanent(_homePageRedirect.HomePage());
        }
    }
}
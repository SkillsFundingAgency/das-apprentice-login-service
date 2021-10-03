using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.BuildLogoutViewModel;
using SFA.DAS.LoginService.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.LoginService.Web.Controllers.Logout
{
    [AllowAnonymous]
    public class LogoutController : BaseController
    {
        public LogoutController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet("Account/Logout")]
        public async Task<IActionResult> Get(string logoutId)
        {
            await SetViewBagClientIdByLogoutId(logoutId);

            var vm = await Mediator.Send(new LogoutRequest {LogoutId = logoutId});
            var result = View("Loggedout", vm);

            if (HttpContext != null && HttpContext.Request.Cookies.Count > 0)
            {
                var cookies = HttpContext.Request.Cookies.Where(x => x.Key.Contains("Apprenticeships") || x.Key.Contains("AspNetCore")).ToList();

                //cookies.ForEach(x => Response.Cookies.Delete(x.Key));
                cookies.ForEach(x =>
                {
                    var options = new CookieOptions { Expires = System.DateTime.Now.AddDays(-1) };
                    this.Response.Cookies.Append(x.Key, x.Value, options);
                });
            }

            return result;
        }
    }
}
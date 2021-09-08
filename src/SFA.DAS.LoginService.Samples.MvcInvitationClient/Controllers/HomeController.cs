using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Samples.MvcInvitationClient.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Samples.MvcInvitationClient.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Invitation() => RedirectToAction("Secure");

        //[HttpPost]
        //public IActionResult Invitation(InvitationModel invitation)
        //{
        //    //var registrationCode = System.Guid.NewGuid();
        //    //var result = new CreateInvitationResponse
        //    //{
        //    //    InvitationId = registrationCode,
        //    //    LoginLink = $"{InvitationService.IdentityServiceHost}/CreateAccount/36bcfaad-1ff7-49cb-8eef-19877b7ad0c9/{registrationCode}",
        //    //};
        //    //return View(new InvitationModel
        //    //{
        //    //    Email = invitation.Email,
        //    //    InvitationResponse = result,
        //    //});
        //    return RedirectToAction("Secure");
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        [Route("/Account/SignIn")]
        public IActionResult Secure()
        {
            var claims = new SecureModel(User.Claims);
            return View(claims);
        }

        [Authorize]
        [Route("/Account/ChangeEmail")]
        public IActionResult ChangeEmail([FromServices] InvitationService inviter)
        {
            return Redirect(inviter.ChangeEmailUri.ToString());
        }

        [Route("/Account/Logout")]
        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}
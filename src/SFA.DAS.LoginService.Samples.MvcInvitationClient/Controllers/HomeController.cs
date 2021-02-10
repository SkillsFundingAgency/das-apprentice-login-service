using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Invitation() => View();

        [HttpPost]
        public async Task<IActionResult> Invitation(
            InvitationModel invitation,
            [FromServices] InvitationService inviter)
        {
            var result = await inviter.Invite(invitation);
            return View(new InvitationModel
            {
                Email = invitation.Email,
                InvitationResponse = result,
            });
        }

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
    }
}
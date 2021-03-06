using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword.ViewModels;

namespace SFA.DAS.LoginService.Web.UnitTests.Controllers.ResetPassword
{

    public class When_Post_called_on_RequestPasswordRequest_with_invalid_ModelState : RequestPasswordResetControllerTestBase
    {
        [Test]
        public async Task Then_RequestPasswordResetRequest_should_not_be_sent()
        {
            Controller.ModelState.AddModelError("Email", "Enter a valid email address");
            await Controller.Post(ClientId, new RequestPasswordResetViewModel {Email = "forgotpassword.com"});
            await Mediator.DidNotReceive().Send(Arg.Any<RequestPasswordResetRequest>());
        }
        
        [Test]
        public async Task Then_ViewResult_is_returned()
        {
            Controller.ModelState.AddModelError("Email", "Enter a valid email address");
            var result = await Controller.Post(ClientId, new RequestPasswordResetViewModel {Email = "forgotpassword.com"});

            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewName.Should().Be("RequestPasswordReset");
            result.As<ViewResult>().Model.Should().BeOfType<RequestPasswordResetViewModel>();
        }
    }
    
    public class WhenPostCalledOnRequestPasswordReset : RequestPasswordResetControllerTestBase
    {
        [Test]
        public async Task Then_Request_is_passed_on_to_mediator()
        {
            await Controller.Post(ClientId, new RequestPasswordResetViewModel{Email = "forgot@password.com"});
            await Mediator.Received().Send(Arg.Is<RequestPasswordResetRequest>(r => r.Email == "forgot@password.com" && r.ClientId == ClientId));
        }

        [Test]
        public async Task Then_RedirectToAction_is_returned()
        {
            var result = await Controller.Post(ClientId, new RequestPasswordResetViewModel{Email = "forgot@password.com"});
            result.Should().BeOfType<RedirectToActionResult>();
            ((RedirectToActionResult) result).ActionName.Should().Be("CodeSent");
        }

        [Test]
        public async Task Then_RouteValues_should_contain_email()
        {
            var result = await Controller.Post(ClientId, new RequestPasswordResetViewModel{Email = "forgot@password.com"});

            ((RedirectToActionResult) result).RouteValues["email"].Should().Be("forgot@password.com");
        }
    }
}
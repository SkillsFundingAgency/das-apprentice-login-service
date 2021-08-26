using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.GetInvitationById;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Types.GetClientById;
using System;

namespace SFA.DAS.LoginService.Web.UnitTests.Controllers.CreatePassword
{
    public class When_GET_CreatePassword_with_valid_Id : CreatePasswordTestsBase
    {
        [Test]
        public void Then_correct_ViewResult_is_returned()
        {
            var invitationId = Guid.NewGuid();
            var clientId = Guid.NewGuid();
            var registrationCode = Guid.NewGuid().ToString();
            Mediator.Send(Arg.Any<GetInvitationByIdRequest>()).Returns(new Invitation
            {
                Id = invitationId,
                SourceId = registrationCode,
                ClientId = clientId,
                IsUserCreated = false,
                ValidUntil = DateTime.MaxValue,
            });
            Mediator.Send(Arg.Any<GetClientByIdRequest>()).Returns(new Client
            {
                ServiceDetails = new Data.JsonObjects.ServiceDetails
                {
                    SupportUrl = "http://confirm.example.com/",
                }
            });

            var result = Controller.Get(invitationId).Result;
            result.Should().BeOfType<RedirectResult>()
                .Which.Url.Should().Be($"http://confirm.example.com/register/{registrationCode}");
        }
    }
}
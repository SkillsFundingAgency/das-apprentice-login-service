using System;
using MediatR;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Types.GetClientById;
using SFA.DAS.LoginService.Web.Controllers.ResetPassword;

namespace SFA.DAS.LoginService.Web.UnitTests.Controllers.ResetPassword
{
    [TestFixture]
    public class RequestPasswordResetControllerTestBase
    {
        protected RequestPasswordResetController Controller;
        protected Guid ClientId;
        protected IMediator Mediator;

        [SetUp]
        public void Arrange()
        {
            Mediator = Substitute.For<IMediator>();
            Controller = new RequestPasswordResetController(Mediator);
            ClientId = Guid.NewGuid();
            Mediator.Send(Arg.Any<GetClientByIdRequest>()).Returns(new Data.Entities.Client
            {
                ServiceDetails = new Data.JsonObjects.ServiceDetails
                {
                    SupportUrl = "https://url/",
                }
            });
        }
    }
}
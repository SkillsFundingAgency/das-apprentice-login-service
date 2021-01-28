using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.CreatePassword;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.UnitTests.CreatePasswordTests
{
    [TestFixture]
    public class CreatePasswordTestsBase
    {
        protected IUserService UserService;
        protected LoginContext LoginContext;
        protected Guid InvitationId;
        protected CreatePasswordHandler Handler;
        protected Guid NewLoginUserId;
        protected ICallbackService CallbackService;

        [SetUp]
        public void SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: "CreatePasswordHandler_tests")
                .Options;

            LoginContext = new LoginContext(dbContextOptions);

            InvitationId = Guid.NewGuid();
            LoginContext.Invitations.Add(new Invitation()
            {
                Id = InvitationId,
                Email = "email@provider.com",
                SourceId = "ABC123",
                Name = "GN1",
                OrganisationName = "ON1",
                ApprenticeshipName = "AN1",
            });
            LoginContext.SaveChanges();

            CallbackService = Substitute.For<ICallbackService>();

            UserService = Substitute.For<IUserService>();
            NewLoginUserId = Guid.NewGuid();

            Handler = new CreatePasswordHandler(UserService, LoginContext, CallbackService);
        }

        [TearDown]
        public async Task Teardown()
        {
            LoginContext.UserLogs.RemoveRange(LoginContext.UserLogs);
            await LoginContext.SaveChangesAsync();
        }
    }
}
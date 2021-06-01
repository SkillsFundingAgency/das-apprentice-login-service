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
        protected IWebUserService UserService;
        protected LoginContext LoginContext;
        protected Guid InvitationId;
        protected Guid SourceId;
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
            SourceId = Guid.NewGuid();
            LoginContext.Invitations.Add(new Invitation()
            {
                Id = InvitationId,
                Email = "email@provider.com",
                SourceId = SourceId.ToString(),
                GivenName = "GN1",
                FamilyName = "FN1",
            });
            LoginContext.SaveChanges();

            CallbackService = Substitute.For<ICallbackService>();

            UserService = Substitute.For<IWebUserService>();
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
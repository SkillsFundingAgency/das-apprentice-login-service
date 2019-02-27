using System;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.UnitTests.Invitations.CreateInvitationHandlerTests
{
    [TestFixture]
    public class CreateInvitationHandlerTestBase
    {
        protected CreateInvitationHandler CreateInvitationHandler;
        protected LoginContext LoginContext; 
        protected ICodeGenerationService CodeGenerationService;
        protected IHashingService HashingService;
        protected IEmailService EmailService;
        protected ILoginConfig LoginConfig;
        protected IUserService UserService;
        protected static Guid ClientId;

        [SetUp]
        public void SetUp()
        {
            BuildLoginContext();

            CodeGenerationService = Substitute.For<ICodeGenerationService>();
            HashingService = Substitute.For<IHashingService>();
            EmailService = Substitute.For<IEmailService>();
            LoginConfig = Substitute.For<ILoginConfig>();
            UserService = Substitute.For<IUserService>();
            
            CreateInvitationHandler = BuildCreateInvitationHandler();
        }

        private void BuildLoginContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: "invitations_tests")
                .Options;

            LoginContext = new LoginContext(dbContextOptions);
            ClientId = Guid.NewGuid();
            LoginContext.Clients.Add(new Client(){Id = ClientId});
            LoginContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            LoginContext.Invitations.RemoveRange(LoginContext.Invitations);
            LoginContext.SaveChanges();
        }
        
        
        protected static CreateInvitationRequest BuildCreateInvitationRequest()
        {
            var createInvitationRequest = new CreateInvitationRequest()
            {
                Email = "invited@email.com",
                GivenName = "InvitedGivenName",
                FamilyName = "InvitedFamilyName",
                SourceId = "InvitedSourceId",
                UserRedirect = new Uri("https://localhost/userredirect"),
                Callback = new Uri("https://localhost/callback"),
                ClientId = ClientId
            };
            return createInvitationRequest;
        }
        
        protected static CreateInvitationRequest BuildEmptyCreateInvitationRequest()
        {
            var createInvitationRequest = new CreateInvitationRequest();
            return createInvitationRequest;
        }

        private CreateInvitationHandler BuildCreateInvitationHandler()
        {
            return new CreateInvitationHandler(LoginContext, CodeGenerationService, HashingService, EmailService, LoginConfig, UserService);
        }
    }
}
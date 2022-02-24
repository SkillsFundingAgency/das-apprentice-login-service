using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.Data.JsonObjects;

namespace SFA.DAS.LoginService.Application.UnitTests.ChangeEmail.StartChangeEmail
{
    [TestFixture]
    public class CreateStartChangeEmailHandlerTestBase
    {
        protected StartChangeEmailHandler Sut;
        protected LoginContext LoginContext; 
        protected IEmailService EmailService;
        protected ILoginConfig LoginConfig;
        protected IWebUserService UserService;
        protected IUserAccountService UserAccountService;
        protected static Guid ClientId;
        protected Guid ChangeEmailTemplateId;
        protected Fixture Fixture = new Fixture();
        protected LoginUser User;
        protected string CurrentUserId = "my_user_id";
        protected string CurrentUserEmail = "current.user@test.com";
        protected string AnotherUserId = "their_user_id";
        protected LoginUser AnotherUser;
        protected string AnotherUserEmail = "another.user@test.com";
        protected string Token = "Token";

        [SetUp]
        public void SetUp()
        {
            BuildLoginContext();

            EmailService = Substitute.For<IEmailService>();
            LoginConfig = Substitute.For<ILoginConfig>();
            LoginConfig.BaseUrl.Returns("https://baseurl");

            User = new LoginUser { GivenName = "Bob", FamilyName = "Hope", Email = CurrentUserEmail };
            AnotherUser = new LoginUser { Email = AnotherUserEmail };
            UserService = Substitute.For<IWebUserService>();
            UserService.FindByEmail(CurrentUserEmail).Returns(Task.FromResult(User));
            UserService.FindById(AnotherUserId).Returns(Task.FromResult(AnotherUser));
            UserService.FindByEmail(AnotherUserEmail).Returns(Task.FromResult(AnotherUser));
            UserService.FindById(CurrentUserId).Returns(Task.FromResult(User));
            UserService.GenerateChangeEmailToken(User, "New@new.com").Returns(Token);

            UserAccountService = Substitute.For<IUserAccountService>();

            Sut = BuildStartChangeEmailHandler();
        }

        private void BuildLoginContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            LoginContext = new LoginContext(dbContextOptions);
            ClientId = Guid.NewGuid();
            ChangeEmailTemplateId = Guid.NewGuid();
            LoginContext.Clients.Add(new Client()
            {
                Id = ClientId, 
                ServiceDetails = new ServiceDetails()
                {
                    ServiceName = "Acme Service", 
                    ServiceTeam = "Acme Service Team",
                    SupportUrl = "https://serviceurl",
                    EmailTemplates = new List<EmailTemplate>(){new EmailTemplate(){Name="ChangeEmailAddress", TemplateId = ChangeEmailTemplateId}}
                }
            });
            LoginContext.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            LoginContext.Invitations.RemoveRange(LoginContext.Invitations);
            LoginContext.UserLogs.RemoveRange(LoginContext.UserLogs);
            LoginContext.SaveChanges();
        }

        protected StartChangeEmailRequest CreateStartChangeEmailRequest()
        {
            return new StartChangeEmailRequest
            {
                ClientId = ClientId,
                UserId = CurrentUserId,
                NewEmailAddress = "New@new.com",
                ConfirmEmailAddress = "New@new.com",
            };
        }

        private StartChangeEmailHandler BuildStartChangeEmailHandler()
        {
            return new StartChangeEmailHandler(UserService, LoginContext, EmailService, LoginConfig);
        }
    }
}
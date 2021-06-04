using System;
using System.Collections.Generic;
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
        protected StartChangeEmailHandler Handler;
        protected LoginContext LoginContext; 
        protected IEmailService EmailService;
        protected ILoginConfig LoginConfig;
        protected IWebUserService UserService;
        protected IUserAccountService UserAccountService;
        protected static Guid ClientId;
        protected Guid ChangeEmailTemplateId;
        protected Fixture Fixture = new Fixture();
        protected LoginUser User;

        [SetUp]
        public void SetUp()
        {
            BuildLoginContext();

            EmailService = Substitute.For<IEmailService>();
            LoginConfig = Substitute.For<ILoginConfig>();
            LoginConfig.BaseUrl.Returns("https://baseurl");
            UserService = Substitute.For<IWebUserService>();
            UserAccountService = Substitute.For<IUserAccountService>();
            User = Fixture.Build<LoginUser>().With(u => u.Email, "A@A.com").Create();

            Handler = BuildStartChangeEmailHandler();
        }

        private void BuildLoginContext()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: "invitations_tests")
                .Options;

            LoginContext = new LoginContext(dbContextOptions);
            ClientId = Guid.NewGuid();
            ChangeEmailTemplateId = Guid.Parse("a2fc2212-253e-47c1-b847-27c10f83f7f5");
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
                NewEmailAddress = "New@new.com",
                ConfirmEmailAddress = "New@new.com",
                CurrentEmailAddress = User.Email
            };
        }


        //protected static CreateInvitationRequest BuildCreateInvitationRequest()
        //{
        //    var createInvitationRequest = new CreateInvitationRequest()
        //    {
        //        Email = "invited@email.com",
        //        GivenName = "InvitedGivenName",
        //        FamilyName = "InvitedFamilyName",
        //        OrganisationName = "InvitedOrganisationName",
        //        ApprenticeshipName = "InvitedApprenticeshipName",
        //        SourceId = "InvitedSourceId",
        //        UserRedirect = new Uri("https://localhost/userredirect"),
        //        Callback = new Uri("https://localhost/callback"),
        //        ClientId = ClientId
        //    };
        //    return createInvitationRequest;
        //}

        //protected static CreateInvitationRequest BuildEmptyCreateInvitationRequest()
        //{
        //    var createInvitationRequest = new CreateInvitationRequest();
        //    return createInvitationRequest;
        //}

        private StartChangeEmailHandler BuildStartChangeEmailHandler()
        {
            return new StartChangeEmailHandler(UserService, LoginContext, EmailService);
        }
    }
}
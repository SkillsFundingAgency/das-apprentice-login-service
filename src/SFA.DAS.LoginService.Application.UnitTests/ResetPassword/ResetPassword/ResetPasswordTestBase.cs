using System;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Data;

namespace SFA.DAS.LoginService.Application.UnitTests.ResetPassword.ResetPassword
{
    [TestFixture]
    public class ResetPasswordTestBase
    {
        protected LoginContext LoginContext;
        protected Guid RequestId;
        protected Guid ClientId;
        protected IWebUserService UserService;
        protected ResetPasswordHandler Handler;
        protected IEmailService EmailService;
        
        [SetUp]
        public void SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            LoginContext = new LoginContext(dbContextOptions);

           
            RequestId = Guid.NewGuid();
            ClientId = Guid.NewGuid();
           
            
            UserService = Substitute.For<IWebUserService>();
            EmailService = Substitute.For<IEmailService>();
            Handler = new ResetPasswordHandler(UserService, LoginContext, EmailService);
        }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.LoginService.Application.ResetPassword;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;

namespace SFA.DAS.LoginService.Application.UnitTests.ResetPassword.CheckPasswordReset
{
    [TestFixture]
    public class CheckPasswordResetTestBase
    {
        protected CheckPasswordResetHandler Handler;
        protected LoginContext LoginContext;
        
        [SetUp]
        public void SetUp()
        {
            var dbContextOptions = new DbContextOptionsBuilder<LoginContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).EnableSensitiveDataLogging()
                .Options;

            LoginContext = new LoginContext(dbContextOptions);
            
            Handler = new CheckPasswordResetHandler(LoginContext);
            
            SystemTime.UtcNow = () => new DateTime(2018,1,1,1,1,0);
        }
    }
}
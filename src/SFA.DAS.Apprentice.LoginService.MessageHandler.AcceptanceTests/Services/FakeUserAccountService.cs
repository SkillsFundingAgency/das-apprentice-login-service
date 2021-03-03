using System;
using System.Threading.Tasks;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services
{
    public class FakeUserAccountService : IUserAccountService
    {
        public Task<LoginUser> FindByEmail(string email)
        {
            if (!email.Contains("new", StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(new LoginUser {Email = email});
            }
            return Task.FromResult((LoginUser)null);
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly LoginUserContext _loginUserContext;

        public UserAccountService(LoginUserContext loginUserContext)
        {
            _loginUserContext = loginUserContext;
        }
        public Task<LoginUser> FindByEmail(string email)
        {
            return _loginUserContext.Users.FirstOrDefaultAsync(x =>
                x.Email != null && x.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}

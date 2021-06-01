using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        private readonly ILogger<CustomPasswordValidator<LoginUser>> _logger;
        private readonly LoginContext _loginContext;

        public CustomPasswordValidator(ILogger<CustomPasswordValidator<LoginUser>> logger, LoginContext loginContext)
        {
            _logger = logger;
            _loginContext = loginContext;
        }
        
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password) ||
                password.Length < 8 ||
                password.All(char.IsDigit) ||
                password.All(char.IsLetter) ||
                !PasswordContainsBothLettersAndDigits(password))
            {
                return IdentityResult.Failed(new IdentityError() { Code = "PasswordValidity", Description = "Password does not meet validity rules" });
            }

            return IdentityResult.Success;
        }

        private bool PasswordContainsBothLettersAndDigits(string password) =>
            password.Any(char.IsDigit) && password.Any(char.IsLetter);
    }
}
using Microsoft.AspNetCore.Identity;

namespace SFA.DAS.LoginService.Data.Entities
{
    public class LoginUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
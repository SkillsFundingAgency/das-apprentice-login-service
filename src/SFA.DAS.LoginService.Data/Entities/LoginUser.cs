using Microsoft.AspNetCore.Identity;
using System;

namespace SFA.DAS.LoginService.Data.Entities
{
    public class LoginUser : IdentityUser
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public Guid RegistrationId { get; set; }
    }
}
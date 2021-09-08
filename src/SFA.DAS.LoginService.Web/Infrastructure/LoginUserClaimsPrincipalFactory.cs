using Microsoft.AspNetCore.Identity;
using SFA.DAS.LoginService.Data.Entities;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public class LoginUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<LoginUser>
    {
        public LoginUserClaimsPrincipalFactory(
            UserManager<LoginUser> userManager,
            Microsoft.Extensions.Options.IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(LoginUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            //identity.AddClaim(new Claim("family_name", user.FamilyName));
            //identity.AddClaim(new Claim("given_name", user.GivenName));
            identity.AddClaim(new Claim("apprentice_id", user.ApprenticeId.ToString()));
            return identity;
        }
    }
}
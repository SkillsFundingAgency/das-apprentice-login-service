using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.LoginService.Samples.MvcInvitationClient.Models
{
    public class SecureModel
    {
        public SecureModel(IEnumerable<System.Security.Claims.Claim> claims)
        {
            Claims = claims?.ToDictionary(x => x.Type, x => x.Value)
                ?? new Dictionary<string, string>();
        }

        public Dictionary<string, string> Claims { get; }
    }
}
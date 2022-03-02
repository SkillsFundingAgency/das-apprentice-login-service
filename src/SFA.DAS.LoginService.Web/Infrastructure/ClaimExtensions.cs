using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Claims;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public static class ClaimExtensions
    {
        public static string Subject(this IEnumerable<Claim> claims)
            => claims.FirstOrDefault(x => x.Type == "sub")?.Value
            ?? throw new InvalidOperationException("No `sub` found in claims");
    }
}

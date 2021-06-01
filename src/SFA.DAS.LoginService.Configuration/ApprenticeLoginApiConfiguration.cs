using SFA.DAS.Http.Configuration;

namespace SFA.DAS.LoginService.Configuration
{
    public class ApprenticeLoginApiConfiguration : IManagedIdentityClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string IdentifierUri { get; set; }
    }
}
using SFA.DAS.LoginService.Application.Interfaces;

namespace SFA.DAS.LoginService.Application.Services
{
    public class StaticCodeGenerator : ICodeGenerator
    {
        public string GenerateAlphaNumeric() => "ABC123";
    }
}

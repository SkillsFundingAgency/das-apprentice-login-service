using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.LoginService.Application.Interfaces;

namespace SFA.DAS.LoginService.Application.Services
{
    public class StaticCodeGenerator : ICodeGenerator
    {
        public string GenerateAlphaNumeric(int length = 6) => "ABC123";
    }
}

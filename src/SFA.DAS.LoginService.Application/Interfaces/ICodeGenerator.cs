using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.LoginService.Application.Interfaces
{
    public interface ICodeGenerator
    {
        string GenerateAlphaNumeric(int length = 6);
    }
}

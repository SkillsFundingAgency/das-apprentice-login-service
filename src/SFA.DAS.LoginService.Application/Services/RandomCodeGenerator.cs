using System;
using System.Linq;
using SFA.DAS.LoginService.Application.Interfaces;

namespace SFA.DAS.LoginService.Application.Services
{
    public class RandomCodeGenerator : ICodeGenerator
    {
        public string GenerateAlphaNumeric()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string nums = "0123456789";

            var part1 = new string(Enumerable.Repeat(chars, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            var part2 = new string(Enumerable.Repeat(nums, 3).Select(s => s[random.Next(s.Length)]).ToArray());
            return part1 + part2;
        }
    }
}

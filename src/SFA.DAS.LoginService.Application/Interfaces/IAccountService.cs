using System.Threading.Tasks;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.Interfaces
{
    public interface IAccountService
    {
        Task<LoginUser> FindByEmail(string email);
    }
}
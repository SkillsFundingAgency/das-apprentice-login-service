using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public interface IConnectionFactory
    {
        DbContextOptionsBuilder AddConnection(DbContextOptionsBuilder builder, string connection);
        DbConnection CreateConnection(string connection);
    }
}

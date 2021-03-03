using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services
{
    public class SqLiteConnectionFactory : IConnectionFactory
    {

        public DbContextOptionsBuilder AddConnection(DbContextOptionsBuilder builder, string connection)
        {
            return builder.UseSqlite(connection);
        }

        public DbConnection CreateConnection(string connection)
        {
            return new SqliteConnection(connection);
        }
    }
}
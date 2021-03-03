using Microsoft.EntityFrameworkCore;
using SFA.DAS.LoginService.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Bindings
{
    [Binding]
    public class Database
    {
        private readonly TestContext _context;

        public Database(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 1)]
        public void Initialise()
        {
            var dbOptions = new DbContextOptionsBuilder<LoginContext>().UseSqlite(_context.DatabaseConnectionString);
            _context.LoginContext = new LoginContext(dbOptions.Options);
            //_context.LoginContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
            _context.LoginContext.Database.EnsureDeleted();
            _context.LoginContext.Database.EnsureCreated();

            //var dbOptions2 = new DbContextOptionsBuilder<LoginUserContext>().UseSqlite(_context.DatabaseConnectionString + "A");
            //_context.LoginUserContext = new LoginUserContext(dbOptions2.Options);
            //_context.LoginUserContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
            //_context.LoginUserContext.Database.EnsureDeleted();
            //_context.LoginUserContext.Database.EnsureCreated();
        }

        [AfterScenario()]
        public void Cleanup()
        {
            _context?.LoginContext?.Database.EnsureDeleted();
            _context?.LoginContext?.Dispose();
            _context?.LoginUserContext?.Dispose();
        }
    }
}
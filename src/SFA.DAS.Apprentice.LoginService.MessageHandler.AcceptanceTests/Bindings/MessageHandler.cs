using System.Threading.Tasks;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Bindings
{
    [Binding]
    public class MessageHandler
    {
        private readonly TestContext _context;

        public MessageHandler(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 3)]
        public async Task InitialiseFunctions()
        {
            _context.MessageHandlerHost = new MessageHandlerTestHost(_context);
            await _context.MessageHandlerHost.Start();
        }

        [AfterScenario()]
        public void CleanUp()
        {
            _context.MessageHandlerHost?.Dispose();
        }
    }
}
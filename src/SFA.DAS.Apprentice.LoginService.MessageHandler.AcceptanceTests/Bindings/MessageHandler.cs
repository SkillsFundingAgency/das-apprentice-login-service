using System.Threading.Tasks;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Bindings
{
    [Binding]
    public class MessageHandler
    {
        public static MessageHandlerTestHost MessageHandlerHost { get; set; }
        private readonly TestContext _context;

        public MessageHandler(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 3)]
        public async Task InitialiseFunctions()
        {
            if (MessageHandlerHost == null)
            {
                MessageHandlerHost = new MessageHandlerTestHost(_context);
                await MessageHandlerHost.Start();
            }

            _context.MessageHandlerHost = MessageHandlerHost;
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            MessageHandlerHost?.Dispose();
            MessageHandlerHost = null;
        }
    }
}
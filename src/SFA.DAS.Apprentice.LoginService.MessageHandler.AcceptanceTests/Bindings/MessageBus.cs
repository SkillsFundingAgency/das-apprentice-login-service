using System.Threading.Tasks;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Bindings
{
    [Binding]
    public class MessageBus
    {
        private readonly TestContext _context;

        public MessageBus(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 2)]
        public async Task InitialiseFunctions()
        {
            _context.TestMessageBus = new TestMessageBus();
            _context.Hooks.Add(new MessageBusHook<MessageContext>());
            await _context.TestMessageBus.Start();
        }
    }
}
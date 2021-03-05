using System;
using System.IO;
using System.Threading.Tasks;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using SQLitePCL;
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
            _context.TestDirectory = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString()));
            if (!_context.TestDirectory.Exists)
            {
                Directory.CreateDirectory(_context.TestDirectory.FullName);
            }

            _context.TestMessageBus = new TestMessageBus();
            _context.Hooks.Add(new MessageBusHook<MessageContext>());
            await _context.TestMessageBus.Start(_context.TestDirectory);
        }

        [AfterScenario()]
        public async Task CleanUp()
        {
            if (_context.TestMessageBus.IsRunning)
            {
                await _context.TestMessageBus.Stop();
            }

            if (_context?.TestDirectory != null && _context.TestDirectory.Exists)
            {
                Directory.Delete(_context.TestDirectory.FullName, true);
            }
        }
    }
}
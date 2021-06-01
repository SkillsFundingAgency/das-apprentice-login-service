using System;
using System.IO;
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
            _context.WorkingDirectory = GetWorkDirectory();
            if (!_context.WorkingDirectory.Exists)
            {
                Directory.CreateDirectory(_context.WorkingDirectory.FullName);
            }

            _context.TestMessageBus = new TestMessageBus();
            _context.Hooks.Add(new MessageBusHook<MessageContext>());
            await _context.TestMessageBus.Start(_context.WorkingDirectory);
        }

        [AfterScenario()]
        public async Task CleanUp()
        {
            if (_context.TestMessageBus.IsRunning)
            {
                await _context.TestMessageBus.Stop();
            }

            if (_context?.WorkingDirectory != null)
            {
                _context.WorkingDirectory.Refresh();
                if (_context.WorkingDirectory.Exists)
                {
                    Directory.Delete(_context.WorkingDirectory.FullName, true);
                }
            }
        }

        private DirectoryInfo GetWorkDirectory()
        {
            var directory = Directory.GetCurrentDirectory();
            var len = directory.IndexOf(@"\src\", StringComparison.Ordinal);
            var temp = @"temp\" + DateTime.UtcNow.ToString("yyyy-MM-dd hhmmss");

            if (len > 0)
            {
                var srcDirectory = directory.Substring(0, len);
                return new DirectoryInfo(Path.Combine(srcDirectory, temp));
            }
            return new DirectoryInfo(Path.Combine(directory, temp));
        }
    }
}
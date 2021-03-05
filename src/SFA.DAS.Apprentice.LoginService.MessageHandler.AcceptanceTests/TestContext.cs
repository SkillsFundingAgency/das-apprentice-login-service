using System.Collections.Generic;
using System.IO;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests
{
    public class TestContext
    {
        public string LoginServiceBaseUrl { get; set; }
        public TestMessageBus TestMessageBus { get; set; }
        public List<IHook> Hooks { get; } = new List<IHook>();
        public MessageHandlerTestHost MessageHandlerHost { get; set; }
        public MockApi LoginServiceApi { get; set; }
        public DirectoryInfo TestDirectory { get; set; }
    }

    public interface IHook { }

}
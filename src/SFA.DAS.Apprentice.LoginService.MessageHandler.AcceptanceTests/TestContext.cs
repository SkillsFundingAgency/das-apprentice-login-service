using System.Collections.Generic;
using SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services;
using SFA.DAS.LoginService.Data;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests
{
    public class TestContext
    {
        public TestContext()
        {
            DatabaseConnectionString = "Data Source=AcceptanceTestDb";
            LoginBaseUrl = "https://login/";
        }
        public string DatabaseConnectionString { get; set; }
        public string LoginBaseUrl { get; set; }
        public TestMessageBus TestMessageBus { get; set; }
        public List<IHook> Hooks { get; } = new List<IHook>();
        public MessageHandlerTestHost MessageHandlerHost { get; set; }
        public LoginUserContext LoginUserContext { get; set; }
        public LoginContext LoginContext { get; set; }
    }

    public interface IHook { }

}
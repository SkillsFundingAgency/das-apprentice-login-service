using TechTalk.SpecFlow;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Bindings
{
    [Binding]
    public class LoginServiceApi
    {
        public static MockApi Client { get; set; }

        private readonly TestContext _context;

        public LoginServiceApi(TestContext context)
        {
            _context = context;
        }

        [BeforeScenario(Order = 1)]
        public void Initialise()
        {
            Client ??= new MockApi();
            _context.LoginServiceApi = Client;
            _context.LoginServiceBaseUrl = Client.BaseAddress;
        }

        [AfterScenario()]
        public static void CleanUp()
        {
            Client?.Reset();
        }

        [AfterFeature()]
        public static void CleanUpFeature()
        {
            Client?.Dispose();
            Client = null;
        }
    }
}
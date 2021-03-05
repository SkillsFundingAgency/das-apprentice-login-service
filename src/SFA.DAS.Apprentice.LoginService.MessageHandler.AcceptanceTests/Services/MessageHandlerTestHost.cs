using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using NServiceBus.Transport;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.AcceptanceTests.Services
{
    public class MessageHandlerTestHost : IDisposable
    {
        private bool _isDisposed;
        private IHost _host;
        private readonly TestContext _context;
        private readonly Dictionary<string, string> _hostConfig = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _appConfig;

        public MessageHandlerTestHost(TestContext context)
        {
            _isDisposed = false;
            _context = context;
            _appConfig = new Dictionary<string, string>
            {
                {"EnvironmentName", "ACCEPTANCE_TESTS"},
                {"ConfigurationStorageConnectionString", "UseDevelopmentStorage=true"},
                {"ConfigNames", "SFA.DAS.EmployerIncentives.Functions"},
                {"NServiceBusConnectionString", "UseDevelopmentStorage=true"},
                {"AzureWebJobsStorage", "UseDevelopmentStorage=true"},
                {"SFA.DAS.ApprenticeLoginService:BaseUrl", _context.LoginServiceBaseUrl},
                {"SFA.DAS.ApprenticeLoginService:ApprenticeLoginApi:ApiBaseUrl", _context.LoginServiceBaseUrl}
            };
        }

        public async Task Start()
        {
            var startUp = new Startup();

            var hostBuilder = new HostBuilder()
                    .ConfigureHostConfiguration(a =>
                    {
                        a.Sources.Clear();
                        a.AddInMemoryCollection(_hostConfig);
                    })
                    .ConfigureAppConfiguration(a =>
                    {
                        a.Sources.Clear();
                        a.AddInMemoryCollection(_appConfig);
                    })
                    .ConfigureWebJobs(c =>
                    {
                    })
                    .ConfigureWebJobs(startUp.Configure)
                ;

            hostBuilder.ConfigureServices((s) =>
            {
                s.AddNServiceBus<MessageHandlerTestHost>(o =>
                {
                    o.EndpointConfiguration = (endpoint) =>
                    {
                        endpoint.UseTransport<LearningTransport>();
                        return endpoint;
                    };

                    if (_context.Hooks.SingleOrDefault(h => h is MessageBusHook<MessageContext>) is MessageBusHook<MessageContext> hook)
                    {
                        o.OnMessageReceived += (message) => hook?.OnReceived?.Invoke(message);
                        o.OnMessageProcessed += (message) => hook?.OnProcessed?.Invoke(message);
                        o.OnMessageErrored += (exception, message) => hook?.OnErrored?.Invoke(exception, message);
                    }
                });
            });

            hostBuilder.UseEnvironment("LOCAL");
            _host = await hostBuilder.StartAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _host?.Dispose();
            }

            _isDisposed = true;
        }
    }
}

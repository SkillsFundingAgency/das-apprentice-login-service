using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure;
using SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure.NServiceBus;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.EmailService;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(SFA.DAS.Apprentice.LoginService.MessageHandler.Startup))]

namespace SFA.DAS.Apprentice.LoginService.MessageHandler
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true);

            var preConfig = builder.ConfigurationBuilder.Build();

            builder.ConfigurationBuilder.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = preConfig["ConfigNames"].Split(",");
                options.StorageConnectionString = preConfig["ConfigurationStorageConnectionString"];
                options.EnvironmentName = preConfig["EnvironmentName"];
                options.PreFixConfigurationKeys = true;
            });
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.ConfigureLogging();
            builder.ConfigureNServiceBus();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            builder.Services
                .AddOptions<LoginConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind("SFA.DAS.ApprenticeLoginService", settings));

            builder.Services.AddSingleton<ILoginConfig>(s =>
                s.GetRequiredService<IOptions<LoginConfig>>().Value);

            builder.Services.AddMediatR(typeof(CreateInvitationHandler).Assembly);
            if (configuration.IsLocalAcceptanceOrDev())
            {
                builder.Services.AddTransient<IEmailService, DevEmailService>();
            }
            else
            {
                builder.Services.AddTransient<IEmailService, EmailService>();
            }
            builder.Services.AddTransient<IUserAccountService, UserAccountService>();
            builder.Services.AddTransient<IClientService, ClientService>();
            builder.Services.AddTransient<IConnectionFactory, SqlServerConnectionFactory>();

            builder.Services.AddDbContext<LoginContext>((services, options) =>
            {
                var connectionFactory = services.GetRequiredService<IConnectionFactory>();
                var config = services.GetRequiredService<ILoginConfig>();
                var loggerFactory = services.GetService<ILoggerFactory>();
                options.UseDataStorage(connectionFactory, config.SqlConnectionString).UseLocalSqlLogger(loggerFactory, configuration);
            });

            builder.Services.AddDbContext<LoginUserContext>((services, options) =>
            {
                var connectionFactory = services.GetRequiredService<IConnectionFactory>();
                var config = services.GetRequiredService<ILoginConfig>();
                var loggerFactory = services.GetService<ILoggerFactory>();
                options.UseDataStorage(connectionFactory, config.SqlConnectionString).UseLocalSqlLogger(loggerFactory, configuration);
            });

        }
    }
}

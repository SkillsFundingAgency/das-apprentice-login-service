using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
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
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.EmailService;
using System.Data.SqlClient;

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

            //builder.Services.ConfigureOptions<LoginConfig>();

            builder.Services
                .AddOptions<LoginConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.Bind("SFA.DAS.ApprenticeLoginService", settings));


            builder.Services.AddSingleton<ILoginConfig>(s =>
                s.GetRequiredService<IOptions<LoginConfig>>().Value);

            //builder.Services.WireUpDependencies();
            builder.Services.AddMediatR(typeof(CreateInvitationHandler).Assembly);
            builder.Services.AddTransient<IEmailService, DevEmailService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IClientService, ClientService>();
            builder.Services.AddTransient<SignInManager<LoginUser>>();
            builder.Services.AddHttpClient<ICallbackService, CallbackService>();


            builder.Services.AddDbContext<LoginContext>((services, options) =>
                    options.Secure(services.GetRequiredService<ILoginConfig>().SqlConnectionString));

            builder.Services.AddDbContext<LoginUserContext>((services, options) =>
                    options.Secure(services.GetRequiredService<ILoginConfig>().SqlConnectionString));
        }
    }

    public static class R
    {

        public static DbContextOptionsBuilder Secure(this DbContextOptionsBuilder builder, string sqlConnectionString)
        {
            var sqlConnection = new SqlConnection(sqlConnectionString);

            //if (!_environment.IsDevelopment())
            //{
            //    sqlConnection.AccessToken = GetAccessToken();
            //}

            builder.UseSqlServer(sqlConnection);

            return builder;
        }
    }



}

using SFA.DAS.LoginService.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.EmailService;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class DependenciesStartupExtensions
    {

        public static void ConfigureFromOptions<TInterface, TOptions>(this IServiceCollection services)
            where TInterface : class
            where TOptions : class, TInterface, new()
        {
            services.AddSingleton<TInterface>(s =>
                s.GetRequiredService<IOptions<TOptions>>().Value);
        }



        public static IServiceCollection WireUpDependencies(this IServiceCollection services, /*ILoginConfig loginConfig, */IConfiguration configuration, IHostingEnvironment environment ) //, IServiceProvider serviceProvider)
        {
            services.AddMediatR(typeof(CreateInvitationHandler).Assembly);

            services.AddTransient<IConfigurationService, ConfigurationService>();

            //loginConfig = new ConfigurationService(serviceProvider.GetService<IMediator>())
            //    .GetLoginConfig(
            //        configuration["EnvironmentName"],
            //        configuration["ConfigurationStorageConnectionString"],
            //        "1.0",
            //        "SFA.DAS.ApprenticeLoginService", environment).Result;

            //services.AddTransient(sp => loginConfig);

            if (environment.IsDevelopment())
            {
                services.AddTransient<IEmailService, DevEmailService>();
            }
            else
            {
                services.AddTransient<IEmailService, EmailService>();
            }

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<SignInManager<LoginUser>>();
            services.AddHttpClient<ICallbackService, CallbackService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(GetRetryPolicy());

            return services;
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            var jitterer = new Random();
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                    + TimeSpan.FromMilliseconds(jitterer.Next(0, 100)));
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data.Entities;
using SFA.DAS.LoginService.EmailService;
using System;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.Infrastructure
{
    public static class DependenciesStartupExtensions
    {
        public static IServiceCollection WireUpDependencies(
            this IServiceCollection services, FunctionsHostBuilderContext context)
        {
            services.AddMediatR(typeof(CreateInvitationHandler).Assembly);

            services.AddTransient(typeof(IEmailService), EmailServiceType(context));
            services.AddTransient(typeof(IContextSecurityProvider), DbSecurityType(context));
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IUserAccountService, UserAccountService>();
            services.AddTransient<IWebUserService, UserService>();
            services.AddTransient<IClientService, ClientService>();
            services.AddTransient<SignInManager<LoginUser>>();

            return services;
        }

        private static Type EmailServiceType(FunctionsHostBuilderContext context) =>
            context.EnvironmentName == EnvironmentName.Development
                ? typeof(DevEmailService)
                : typeof(EmailService);

        private static Type DbSecurityType(FunctionsHostBuilderContext context) =>
            context.EnvironmentName == EnvironmentName.Development
                ? typeof(DevSecurityProvider)
                : typeof(ManagedIdentityAzureTokenProvider);
    }
}
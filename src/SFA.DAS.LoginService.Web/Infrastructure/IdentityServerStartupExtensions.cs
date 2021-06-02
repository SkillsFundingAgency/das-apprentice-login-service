using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;

namespace SFA.DAS.LoginService.Web.Infrastructure
{
    public static class IdentityServerStartupExtensions
    {
        public static void AddIdentityServer(
            this IServiceCollection services,
            ILoginConfig loginConfig,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
            ILogger logger)
        {
            services.AddIdentity<LoginUser, IdentityRole>(
                     options =>
                     {
                         options.Password.RequireNonAlphanumeric = false;
                         options.Password.RequireLowercase = false;
                         options.Password.RequireUppercase = false;
                         options.Lockout.MaxFailedAccessAttempts = loginConfig.MaxFailedAccessAttempts;
                         options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(14);
                     })
                .AddPasswordValidator<CustomPasswordValidator<LoginUser>>()
                .AddClaimsPrincipalFactory<LoginUserClaimsPrincipalFactory>()
                .AddEntityFrameworkStores<LoginUserContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<ManagedIdentityAzureTokenProvider>();
            services.AddSingleton<IContextSecurityProvider>(s =>
                s.GetRequiredService<ManagedIdentityAzureTokenProvider>());

            services.AddTransient<ICodeGenerator, RandomCodeGenerator>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = ".Login.Identity.Application";
                options.Cookie.HttpOnly = true;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            var securityProvider = services.BuildServiceProvider()
                .GetRequiredService<IContextSecurityProvider>();

            var isBuilder = services
                .AddIdentityServer(options =>
                {
                    options.UserInteraction.ErrorUrl = "/Error";
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        securityProvider.Secure(builder, loginConfig.SqlConnectionString);
                    options.DefaultSchema = "IdentityServer";
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        securityProvider.Secure(builder, loginConfig.SqlConnectionString);
                    options.DefaultSchema = "IdentityServer";
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<LoginUser>();

            if (environment.IsDevelopment())
            {
                isBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                isBuilder.AddCertificateFromStore(loginConfig.CertificateThumbprint, logger);
            }
        }
    }
}
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NServiceBus;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;
using SFA.DAS.Notifications.Messages.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IMessageSession _messageSession;

        public EmailService(IMessageSession messageSession, ILogger<EmailService> logger)
        {
            _logger = logger;
            _messageSession = messageSession;
        }

        public async Task SendInvitationEmail(InvitationEmailViewModel viewModel)
        {
            await SendEmail(viewModel);
        }

        public async Task SendResetPassword(ResetPasswordEmailViewModel viewModel)
        {
            await SendEmail(viewModel);
        }

        public async Task SendResetNoAccountPassword(PasswordResetNoAccountEmailViewModel viewModel)
        {
            await SendEmail(viewModel);
        }

        public async Task SendPasswordReset(PasswordResetEmailViewModel viewModel)
        {
            await SendEmail(viewModel);
        }

        public async Task SendUserExistsEmail(UserExistsEmailViewModel viewModel)
        {
            await SendEmail(viewModel);
        }

        private async Task SendEmail(EmailViewModel viewModel)
        {
            _logger.LogInformation($"CreateInvitationHandler : SendEmail : ViewModel: {JsonConvert.SerializeObject(viewModel)}");

            var tokens = GetTokens(viewModel);

            await _messageSession.Send(
                new SendEmailCommand(
                    viewModel.TemplateId.ToString(),
                    viewModel.EmailAddress,
                    tokens));
        }

        private Dictionary<string, string> GetTokens(EmailViewModel vm)
        {
            _logger.LogInformation($"CreateInvitationHandler : GetTokens : ViewModel: {JsonConvert.SerializeObject(vm)}");

            return vm.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.GetValue(vm) != null)
                .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(vm).ToString());
        }
    }
}
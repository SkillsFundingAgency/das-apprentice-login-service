using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages.Commands;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.ChangeEmail.ConfirmChangeEmail
{
    public class ConfirmChangeEmailRequestHandler : IRequestHandler<ConfirmChangeEmailRequest, ConfirmChangeEmailResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;
        private readonly IMessageSession _messageSession;
        private readonly ILogger<ConfirmChangeEmailRequestHandler> _logger;

        public ConfirmChangeEmailRequestHandler(IWebUserService userService, LoginContext loginContext, IMessageSession messageSession, ILogger<ConfirmChangeEmailRequestHandler> logger)
        {
            _userService = userService;
            _loginContext = loginContext;
            _messageSession = messageSession;
            _logger = logger;
        }

        public async Task<ConfirmChangeEmailResponse> Handle(ConfirmChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = ValidatedRequest(request);
            if (response.HasErrors) return response;

            var user = await _userService.FindById(request.SubjectId);

            if (user == null)
            {
                response.TokenError = true;
                return response;
            }

            _logger.LogInformation($"About to change email for apprentice {user.ApprenticeId}");

            var result = await _userService.ChangeEmail(user, request.Password, request.NewEmailAddress, request.Token);
            if (!result.Succeeded)
            {
                _logger.LogInformation($"ChangeEmail for apprentice {user.ApprenticeId}, returned some errors");
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                    response.PasswordError = "Incorrect password";
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidToken)))
                    response.TokenError = true;

                return response;
            }

            _logger.LogInformation($"Event {nameof(UpdateEmailAddressCommand)} getting published for apprentice {user.ApprenticeId}");
            await _messageSession.Send(new UpdateEmailAddressCommand
            {
                ApprenticeId = user.ApprenticeId,
                NewEmailAddress = request.NewEmailAddress,
                CurrentEmailAddress = request.CurrentEmailAddress
            });

            _logger.LogInformation($"Logging 'Confirm Change Email' for apprentice {user.ApprenticeId}");
            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(),
                Action = "Confirm Change Email",
                Email = request.CurrentEmailAddress,
                Result = "Change Users email confirmed",
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress
            });

            await _loginContext.SaveChangesAsync(cancellationToken);

            return new ConfirmChangeEmailResponse();
        }

        private ConfirmChangeEmailResponse ValidatedRequest(ConfirmChangeEmailRequest request)
        {
            var response = new ConfirmChangeEmailResponse();
            if (string.IsNullOrEmpty(request.Password))
                response.PasswordError = "Password cannot be blank";
            return response;
        }
    }
}
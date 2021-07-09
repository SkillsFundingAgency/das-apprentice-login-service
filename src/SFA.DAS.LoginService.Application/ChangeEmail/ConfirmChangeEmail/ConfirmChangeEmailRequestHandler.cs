using MediatR;
using Microsoft.AspNetCore.Identity;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Apprentice.LoginService.Messages;

namespace SFA.DAS.LoginService.Application.ChangeEmail.ConfirmChangeEmail
{
    public class ConfirmChangeEmailRequestHandler : IRequestHandler<ConfirmChangeEmailRequest, ConfirmChangeEmailResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;
        private readonly IMessageSession _messageSession;

        public ConfirmChangeEmailRequestHandler(IWebUserService userService, LoginContext loginContext, IMessageSession messageSession)
        {
            _userService = userService;
            _loginContext = loginContext;
            _messageSession = messageSession;
        }

        public async Task<ConfirmChangeEmailResponse> Handle(ConfirmChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = ValidatedRequest(request);
            if (response.HasErrors) return response;

            var user = await _userService.FindByEmail(request.CurrentEmailAddress);

            if (user == null)
            {
                response.TokenError = true;
                return response;
            }

            var result = await _userService.ChangeEmail(user, request.Password, request.NewEmailAddress, request.Token);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                    response.PasswordError = "Incorrect password";
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidToken)))
                    response.TokenError = true;

                return response;
            }
            
            await _messageSession.Publish(new EmailChangedEvent
            {
                ApprenticeId = user.ApprenticeId, 
                NewEmailAddress = request.NewEmailAddress,
                CurrentEmailAddress = request.CurrentEmailAddress
            });

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(),
                Action = "Confirm Change Email",
                Email = request.CurrentEmailAddress,
                Result = "Change Users email confirmed",
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress
            });

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
using MediatR;
using Microsoft.AspNetCore.Identity;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class ConfirmChangeEmailRequestHandler : IRequestHandler<ConfirmChangeEmailRequest, ConfirmChangeEmailResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;

        public ConfirmChangeEmailRequestHandler(IWebUserService userService, LoginContext loginContext)
        {
            _userService = userService;
            _loginContext = loginContext;
        }

        public async Task<ConfirmChangeEmailResponse> Handle(ConfirmChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = ValidatedRequest(request);
            if (response.HasErrors) return response;

            var user = await _userService.FindByEmail(request.CurrentEmailAddress)
                ?? throw new ApplicationException(
                    $"Current User's email `{request.CurrentEmailAddress}` does not exist");

            var result = await _userService.ChangeEmail(user, request.Password, request.NewEmailAddress, request.Token);
            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordMismatch)))
                    response.PasswordError = "Incorrect password";
                if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidToken)))
                    response.TokenError = true;
            }

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(),
                Action = "Confirm Change Email",
                Email = request.CurrentEmailAddress,
                Result = "Change Users email confirmed",
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress
            });

            return response;
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
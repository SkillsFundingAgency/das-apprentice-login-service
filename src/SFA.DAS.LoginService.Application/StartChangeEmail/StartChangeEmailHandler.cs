using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class StartChangeEmailHandler : IRequestHandler<StartChangeEmailRequest, StartChangeEmailResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;
        private readonly ICodeGenerator _codeGenerator;
        private readonly IEmailService _emailService;

        public StartChangeEmailHandler(IWebUserService userService, LoginContext loginContext, ICodeGenerator codeGenerator, IEmailService emailService)
        {
            _userService = userService;
            _loginContext = loginContext;
            _codeGenerator = codeGenerator;
            _emailService = emailService;
        }

        public async Task<StartChangeEmailResponse> Handle(StartChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = new StartChangeEmailResponse();

            if (ValidatedRequest(request, response))
            {
                return response;
            }

            var user = await _userService.FindByEmail(request.CurrentEmailAddress);
            if (user is null)
            {
                throw new ApplicationException($"Current Users email {request.CurrentEmailAddress} does not exist");
            }

            await SendChangeEmailCode(request);

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(), 
                Action = "Start Change Email", 
                Email = "request.Email", 
                Result = "Change Users email started", 
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress 
            });
            
            return new StartChangeEmailResponse();
        }

        private async Task SendChangeEmailCode(StartChangeEmailRequest request)
        {
            var code = _codeGenerator.GenerateAlphaNumeric(6);
            var templateCode = Guid.NewGuid();

            await _emailService.SendChangeEmailCode(new ChangeUserEmailViewModel
            {
                Code = code,
                EmailAddress = request.NewEmailAddress,
                Subject = "Change your email",
                TemplateId = templateCode
            });
        }

        private bool ValidatedRequest(StartChangeEmailRequest request, StartChangeEmailResponse response)
        {
            bool hasError = false;

            if (!request.NewEmailAddress.Contains("@"))
            {
                hasError = true;
                response.NewEmailAddressError = "Must be a valid email address";
            }

            if (!request.ConfirmEmailAddress.Contains("@"))
            {
                hasError = true;
                response.ConfirmEmailAddressError = "Must be a valid email address";
            }

            if (hasError)
            {
                return true;
            }

            if (!request.NewEmailAddress.Equals(request.ConfirmEmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                response.NewEmailAddressError = "Email addresses must match";
                return true;
            }

            if (request.NewEmailAddress.Equals(request.CurrentEmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                response.NewEmailAddressError = "This email is the same as you're current email address";
                return true;
            }

            return false;
        }
    }
}
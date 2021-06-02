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
            var response = ValidatedRequest(request);

            if(response.HasErrors)
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
                Email = request.CurrentEmailAddress, 
                Result = "Change Users email started", 
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress 
            });
            
            return new StartChangeEmailResponse();
        }

        private async Task SendChangeEmailCode(StartChangeEmailRequest request)
        {
            var code = _codeGenerator.GenerateAlphaNumeric();
            var templateCode = Guid.NewGuid();

            await _emailService.SendChangeEmailCode(new ChangeUserEmailViewModel
            {
                Code = code,
                EmailAddress = request.NewEmailAddress,
                Subject = "Change your email",
                TemplateId = templateCode
            });
        }

        private StartChangeEmailResponse ValidatedRequest(StartChangeEmailRequest request)
        {
            var response = new StartChangeEmailResponse();

            if (string.IsNullOrWhiteSpace(request.NewEmailAddress))
            {
                response.NewEmailAddressError = "Email address cannot be blank";
            }
            else if (!request.NewEmailAddress.Contains("@"))
            {
                response.NewEmailAddressError = "Must be a valid email address";
            }

            if (string.IsNullOrWhiteSpace(request.ConfirmEmailAddress))
            {
                response.NewEmailAddressError = "Email address cannot be blank";
            }
            else if(!request.ConfirmEmailAddress.Contains("@"))
            {
                response.ConfirmEmailAddressError = "Must be a valid email address";
            }

            if (response.HasErrors)
            {
                return response;
            }

            if (!request.NewEmailAddress.Equals(request.ConfirmEmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                response.NewEmailAddressError = "Email addresses must match";
                return response;
            }

            if (request.NewEmailAddress.Equals(request.CurrentEmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                response.NewEmailAddressError = "This email is the same as you're current email address";
                return response;
            }

            return response;
        }
    }
}
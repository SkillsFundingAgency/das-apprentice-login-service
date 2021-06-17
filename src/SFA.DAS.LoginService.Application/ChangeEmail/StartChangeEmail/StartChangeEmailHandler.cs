using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Application.Services.EmailServiceViewModels;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail
{
    public class StartChangeEmailHandler : IRequestHandler<StartChangeEmailRequest, StartChangeEmailResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;
        private readonly IEmailService _emailService;

        public StartChangeEmailHandler(IWebUserService userService, LoginContext loginContext, IEmailService emailService)
        {
            _userService = userService;
            _loginContext = loginContext;
            _emailService = emailService;
        }

        public async Task<StartChangeEmailResponse> Handle(StartChangeEmailRequest request, CancellationToken cancellationToken)
        {
            request.NewEmailAddress = request.NewEmailAddress?.Trim();
            request.ConfirmEmailAddress = request.ConfirmEmailAddress?.Trim();

            var response = await ValidatedRequest(request);

            if (response.HasErrors)
            {
                return response;
            }

            var user = await _userService.FindByEmail(request.CurrentEmailAddress);
            if (user is null)
            {
                throw new ApplicationException($"Current Users email {request.CurrentEmailAddress} does not exist");
            }

            await SendChangeEmailLink(request, user, cancellationToken);

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(),
                Action = "Start Change Email",
                Email = request.CurrentEmailAddress,
                Result = "Change Users email started",
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress
            });

            await _loginContext.SaveChangesAsync();

            return new StartChangeEmailResponse();
        }

        private async Task SendChangeEmailLink(StartChangeEmailRequest request, LoginUser user,
            CancellationToken cancellationToken)
        {
            async Task<string> BuildLink(string baseUrl)
            {
                string startChar = null;
                var code = await _userService.GenerateChangeEmailToken(user, request.NewEmailAddress);

                if (!baseUrl.EndsWith("/"))
                {
                    startChar = "/";
                }

                return $"{baseUrl}{startChar}profile/{request.ClientId}/changeemail/confirm?email={HttpUtility.UrlEncode(request.NewEmailAddress)}&token={HttpUtility.UrlEncode(code)}";
            }

            var client = await _loginContext.Clients.SingleAsync(c => c.Id == request.ClientId, cancellationToken);
            var templateId = client.ServiceDetails.EmailTemplates.SingleOrDefault(t => t.Name == "ChangeEmailAddress")?.TemplateId;
            if (templateId == null)
            {
                throw new ApplicationException("No email template id found for Change Email Address");
            }

            await _emailService.SendChangeEmailCode(new ChangeUserEmailViewModel
            {
                ConfirmEmailLink = await BuildLink(client.ServiceDetails.SupportUrl),
                GivenName = user.GivenName,
                FamilyName = user.FamilyName,
                EmailAddress = request.NewEmailAddress,
                Subject = "Confirm your new email address",
                TemplateId = templateId.Value
            });
        }

        private async Task<StartChangeEmailResponse> ValidatedRequest(StartChangeEmailRequest request)
        {
            var response = new StartChangeEmailResponse();

            if (string.IsNullOrWhiteSpace(request.NewEmailAddress))
            {
                response.NewEmailAddressError = "Email address cannot be blank";
            }
            else if (!IsValidEmail(request.NewEmailAddress))
            {
                response.NewEmailAddressError = "Must be a valid email address";
            }

            if (string.IsNullOrWhiteSpace(request.ConfirmEmailAddress))
            {
                response.ConfirmEmailAddressError = "Email address cannot be blank";
            }
            else if (!IsValidEmail(request.ConfirmEmailAddress))
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
                response.ConfirmEmailAddressError = "Email addresses must match";
                return response;
            }

            if (request.NewEmailAddress.Equals(request.CurrentEmailAddress,
                StringComparison.InvariantCultureIgnoreCase))
            {
                response.NewEmailAddressError = "This email is the same as your current email address";
                return response;
            }

            var user = await _userService.FindByEmail(request.NewEmailAddress);
            if (user != null)
            {
                response.NewEmailAddressError = "This email is already in use by another account";
                return response;
            }

            return response;
        }

        private bool IsValidEmail(string emailAsString)
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(emailAsString);

                // check it contains a top level domain
                var parts = email.Address.Split('@');
                if (!parts[1].Contains(".") || parts[1].EndsWith("."))
                {
                    return false;
                }

                return email.Address == emailAsString;
            }
            catch
            {
                return false;
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LoginService.Application.CreatePassword;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class StartChangeEmailHandler : IRequestHandler<StartChangeEmailRequest, StartChangeEmailResponse>
    {
        private readonly IUserService _userService;
        private readonly LoginContext _loginContext;
        private readonly ICallbackService _callbackService;

        public StartChangeEmailHandler(IUserService userService, LoginContext loginContext, ICallbackService callbackService)
        {
            _userService = userService;
            _loginContext = loginContext;
            _callbackService = callbackService;
        }

        public async Task<StartChangeEmailResponse> Handle(StartChangeEmailRequest request, CancellationToken cancellationToken)
        {
            var response = new StartChangeEmailResponse();

            if (ValidatedRequest(request, response))
            {
                return response;
            }

            var user = await _userService.FindByEmail(request.CurrentEmailAddress);

            var token = await _userService.GenerateChangeEmailToken(user, request.NewEmailAddress);

            //var invitation = await _loginContext.Invitations.SingleOrDefaultAsync(i => i.Id == request.InvitationId, cancellationToken: cancellationToken);

            //var newUserResponse = await _userService.CreateUser(
            //    new LoginUser()
            //    {
            //        UserName = invitation.Email,
            //        Email = invitation.Email,
            //        GivenName = invitation.GivenName,
            //        FamilyName = invitation.FamilyName,
            //        RegistrationId = Guid.TryParse(invitation.SourceId, out var rid) ? rid : default,
            //    }, request.NewEmailAddress);

            //if (newUserResponse.Result != IdentityResult.Success)
            //{
            //    return new StartChangeEmailResponse(){PasswordValid = false};    
            //}
            
            //invitation.IsUserCreated = true;

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(), 
                Action = "Change Email", 
                Email = "request.Email", 
                Result = "User account created", 
                DateTime = SystemTime.UtcNow(),
                ExtraData = request.NewEmailAddress 
            });
            
            await _loginContext.SaveChangesAsync(cancellationToken);
            
            return new StartChangeEmailResponse();
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
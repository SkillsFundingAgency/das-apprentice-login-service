using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.ResetPassword
{
    public class RequestPasswordResetHandler : IRequestHandler<RequestPasswordResetRequest>
    {
        private readonly IEmailService _emailService;
        private readonly ILoginConfig _loginConfig;
        private readonly LoginContext _loginContext;
        private readonly IUserService _userService;

        public RequestPasswordResetHandler(IEmailService emailService,
            ILoginConfig loginConfig, LoginContext loginContext, IUserService userService)
        {
            _emailService = emailService;
            _loginConfig = loginConfig;
            _loginContext = loginContext;
            _userService = userService;
        }

        public async Task<Unit> Handle(RequestPasswordResetRequest request, CancellationToken cancellationToken)
        {
            var loginUser = await _userService.FindByEmail(request.Email);
            if (loginUser == null)
            {
                var client = await _loginContext.Clients.SingleOrDefaultAsync(c => c.Id == request.ClientId, cancellationToken);
                await _emailService.SendResetNoAccountPassword(request.Email, client.ServiceDetails.PostPasswordResetReturnUrl);    
                return Unit.Value;
            }

            await ClearOutAnyPreviousStillValidRequests(request.Email);

            var identityToken = await _userService.GeneratePasswordResetToken(loginUser);

            var resetPasswordRequest = await SavePasswordRequest(request, cancellationToken, identityToken);

            var resetUri = new Uri(new Uri(_loginConfig.BaseUrl), $"NewPassword/{request.ClientId}/{resetPasswordRequest.Id}");
            
            await _emailService.SendResetPassword(request.Email, resetUri.ToString());
            return Unit.Value;
        }

        private async Task ClearOutAnyPreviousStillValidRequests(string email)
        {
            var stillValidRequests = await _loginContext.ResetPasswordRequests.Where(r => r.ValidUntil > SystemTime.UtcNow() && r.IsComplete == false).ToListAsync();
            stillValidRequests.ForEach(r => r.ValidUntil = SystemTime.UtcNow().AddDays(-1));
            await _loginContext.SaveChangesAsync();
        }

        private async Task<ResetPasswordRequest> SavePasswordRequest(RequestPasswordResetRequest request, CancellationToken cancellationToken,
            string identityToken)
        {
            var resetPasswordRequest = new ResetPasswordRequest()
            {
                ClientId = request.ClientId,
                IsComplete = false,
                ValidUntil = SystemTime.UtcNow().AddHours(_loginConfig.PasswordResetExpiryInHours),
                Email = request.Email,
                RequestedDate = SystemTime.UtcNow(),
                IdentityToken = identityToken
            };
            _loginContext.ResetPasswordRequests.Add(resetPasswordRequest);
            await _loginContext.SaveChangesAsync(cancellationToken);
            return resetPasswordRequest;
        }
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.LoginService.Configuration;
using SFA.DAS.LoginService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.Invitations.GetInvitations
{
    public class GetInvitationsRequest : IRequest<GetInvitationsResponse>
    {
        public GetInvitationsRequest(Guid clientId, string sourceId)
            => (ClientId, SourceId) = (clientId, sourceId);

        public Guid ClientId { get; }
        public string SourceId { get; }
    }

    public class GetInvitationsResponse
    {
        public List<Uri> Invitations { get; private set; }
        public string ErrorMessage { get; private set; }

        internal static GetInvitationsResponse Success(IEnumerable<Uri> invitations)
            => new GetInvitationsResponse { Invitations = invitations.ToList() };

        internal static GetInvitationsResponse Error(string errorMessage)
            => new GetInvitationsResponse { ErrorMessage = errorMessage };
    }

    public class GetInvitationsHandler : IRequestHandler<GetInvitationsRequest, GetInvitationsResponse>
    {
        private readonly LoginContext _loginContext;
        private readonly ILoginConfig _loginConfig;
        private readonly ILogger<GetInvitationsHandler> _logger;

        public GetInvitationsHandler(
            LoginContext loginContext,
            ILoginConfig loginConfig,
            ILogger<GetInvitationsHandler> logger)
        {
            _loginContext = loginContext ?? throw new ArgumentNullException(nameof(loginContext));
            _loginConfig = loginConfig ?? throw new ArgumentNullException(nameof(_loginConfig));
            _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }

        public async Task<GetInvitationsResponse> Handle(
            GetInvitationsRequest request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation($"GetInvitationsHandler : Request received: {JsonConvert.SerializeObject(request)}");

            var client = await _loginContext.Clients
                .SingleOrDefaultAsync(c => c.Id == request.ClientId);

            if (client == null)
            {
                _logger.LogInformation($"GetInvitationsHandler : Client does not exist");
                return GetInvitationsResponse.Error("Client does not exist");
            }

            _logger.LogInformation($"GetInvitationsHandler : Client: {JsonConvert.SerializeObject(client)}");

            var createPasswordUri = new Uri(
                new Uri(_loginConfig.BaseUrl), "Invitations/CreatePassword/");

            var invitations = _loginContext.Invitations
                .Where(x => x.SourceId == request.SourceId)
                .Where(x => !x.IsUserCreated)
                .OrderByDescending(x => x.ValidUntil)
                .Select(x => new Uri(createPasswordUri, x.Id.ToString()));

            return GetInvitationsResponse.Success(invitations);
        }
    }
}
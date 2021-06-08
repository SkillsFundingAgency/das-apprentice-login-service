using MediatR;
using System;

namespace SFA.DAS.LoginService.Application.ChangeEmail.ChangeEmailSuccessful
{
    public class ChangeEmailSuccessfulRequest : IRequest<ChangeEmailSuccessfulResponse>
    {
        public Guid ClientId { get; set; }
    }
}

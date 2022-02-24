using MediatR;
using System;

namespace SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail
{
    public class StartChangeEmailRequest : IRequest<StartChangeEmailResponse>
    {
        public Guid ClientId { get; set; }
        public string UserId { get; set; }
        public string NewEmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }
    }
}
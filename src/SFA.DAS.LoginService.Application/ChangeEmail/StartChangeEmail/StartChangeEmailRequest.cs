using MediatR;
using System;
using System.Security.Claims;

namespace SFA.DAS.LoginService.Application.ChangeEmail.StartChangeEmail
{
    public class StartChangeEmailRequest : IRequest<StartChangeEmailResponse>
    {
        public Guid ClientId { get; set; }
        public string SubjectId { get; set; }
        public string CurrentEmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }
    }
}
using MediatR;

namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class StartChangeEmailRequest : IRequest<StartChangeEmailResponse>
    {
        public string CurrentEmailAddress { get; set; } 
        public string NewEmailAddress { get; set; }
        public string ConfirmEmailAddress { get; set; }
    }
}
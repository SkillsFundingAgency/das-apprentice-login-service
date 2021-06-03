using MediatR;

namespace SFA.DAS.LoginService.Application.StartChangeEmail
{
    public class ConfirmChangeEmailRequest : IRequest<ConfirmChangeEmailResponse>
    {
        public string CurrentEmailAddress { get; set; }
        public string NewEmailAddress { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
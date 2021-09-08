using System;
using MediatR;

namespace SFA.DAS.LoginService.Application.CreateAccount
{
    public class CreateAccountRequest : IRequest<CreateAccountResponse>
    {
        public Guid ClientId { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
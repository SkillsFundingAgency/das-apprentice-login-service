
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Application.ChangeEmail.ChangeEmailSuccessful
{
    class ChangeEmailSuccessfulHandler : IRequestHandler<ChangeEmailSuccessfulRequest, ChangeEmailSuccessfulResponse>
    {
        public Task<ChangeEmailSuccessfulResponse> Handle(ChangeEmailSuccessfulRequest request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

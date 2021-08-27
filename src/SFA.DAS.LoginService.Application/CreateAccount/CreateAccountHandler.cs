using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SFA.DAS.LoginService.Application.Interfaces;
using SFA.DAS.LoginService.Application.Services;
using SFA.DAS.LoginService.Data;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Application.CreateAccount
{
    public class CreateAccountHandler : IRequestHandler<CreateAccountRequest, CreateAccountResponse>
    {
        private readonly IWebUserService _userService;
        private readonly LoginContext _loginContext;

        public CreateAccountHandler(IWebUserService userService, LoginContext loginContext)
        {
            _userService = userService;
            _loginContext = loginContext;
        }

        public async Task<CreateAccountResponse> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
        {
            var newUserResponse = await _userService.CreateUser(
                new LoginUser()
                {
                    UserName = request.Email,
                    Email = request.Email,
                    ApprenticeId = Guid.NewGuid()
                }, request.Password);

            if (newUserResponse.Result != IdentityResult.Success)
            {
                return new CreateAccountResponse() { PasswordValid = false };
            }

            _loginContext.UserLogs.Add(new UserLog()
            {
                Id = GuidGenerator.NewGuid(), 
                Action = "Create password", 
                Result = "User account created", 
                DateTime = SystemTime.UtcNow(),
                Email = request.Email
            });
            
            await _loginContext.SaveChangesAsync(cancellationToken);

            var signInResult = await _userService.SignInUser(request.Email, request.Password, false);

            return new CreateAccountResponse(){PasswordValid = true};
        }
    }
}
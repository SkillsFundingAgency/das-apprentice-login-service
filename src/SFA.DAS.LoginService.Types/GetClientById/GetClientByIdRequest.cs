using System;
using MediatR;
using SFA.DAS.LoginService.Data.Entities;

namespace SFA.DAS.LoginService.Types.GetClientById
{
    public class GetClientByIdRequest : IRequest<Client>
    {
        public Guid ClientId { get; set; }
    }
}
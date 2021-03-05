using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using RestEase;

namespace SFA.DAS.Apprentice.LoginService.MessageHandler.InvitationService
{
    public interface IInvitationApi
    {
        [Post("/Invitations/{clientId}")]
        Task<SendInvitationResponse> SendInvitation([Path]Guid clientId, [Body] SendInvitationRequest email);
    }
}
using IdentityModel.Client;
using Newtonsoft.Json;
using SFA.DAS.LoginService.Application.Invitations.CreateInvitation;
using SFA.DAS.LoginService.Samples.MvcInvitationClient.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Samples.MvcInvitationClient
{
    public class InvitationService
    {
        public const string IdentityServiceHost =
                //*
                "https://localhost:53401"
                /*/
                "https://das-at-aplogin-as.azurewebsites.net"
                //*/
                ;
        
        private const string ClientId = "36BCFAAD-1FF7-49CB-8EEF-19877B7AD0C9";

        internal async Task<CreateInvitationResponse> Invite(InvitationModel invitation)
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(IdentityServiceHost);
            if (disco.IsError) throw new Exception(disco.Error);

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
                Scope = "api1"
            });

            if (tokenResponse.IsError) throw new Exception(tokenResponse.Error);

            using (var httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(tokenResponse.AccessToken);

                var inviteJson = JsonConvert.SerializeObject(new
                {
                    sourceId = Guid.NewGuid().ToString(),
                    givenName = "Bobby",
                    familyName = "Bob",
                    email = invitation.Email,
                    userRedirect = "https://localhost:7070/Account/SignIn",
                    callback = "https://localhost:7070/Account/Callback"
                });

                var response = await httpClient.PostAsync(
                    $"{IdentityServiceHost}/Invitations/{ClientId}",
                    new StringContent(inviteJson, Encoding.UTF8, "application/json")
                                                         );

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<CreateInvitationResponse>(content);
                result.LoginLink = $"{IdentityServiceHost}/Invitations/CreatePassword/{result.InvitationId}";
                return result;
            }
        }

        internal Uri ChangeEmailUri =>
            new Uri($"{IdentityServiceHost}/profile/{ClientId}/changeemail");
    }
}
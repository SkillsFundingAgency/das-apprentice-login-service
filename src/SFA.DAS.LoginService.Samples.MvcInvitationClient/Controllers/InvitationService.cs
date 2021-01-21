using IdentityModel.Client;
using Newtonsoft.Json;
using SFA.DAS.LoginService.Samples.MvcInvitationClient.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.LoginService.Samples.MvcInvitationClient
{
    public class InvitationService
    {
        internal async Task Invite(InvitationModel invitation)
        {
            var client = new HttpClient();
            var disco = client.GetDiscoveryDocumentAsync("https://das-at-aplogin-as.azurewebsites.net/").Result;
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
                    userRedirect = "https://localhost:44385/",
                    callback = "https://localhost:44385/"
                });

                var response = await httpClient.PostAsync(
                    "https://das-at-aplogin-as.azurewebsites.net/Invitations/36BCFAAD-1FF7-49CB-8EEF-19877B7AD0C9",
                    new StringContent(inviteJson, Encoding.UTF8, "application/json")
                                                   );

                var content = await response.Content.ReadAsStringAsync();

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
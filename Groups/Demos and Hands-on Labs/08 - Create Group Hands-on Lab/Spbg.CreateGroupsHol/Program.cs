using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Spbg.CreateGroupsHol
{
    class Program
    {
        // Poor-mans token cache
        private static string _token;

        // 1. Get the values below from your app registration on https://apps.dev.microsoft.com
        private const string ClientId = "";
        private const string ReplyUri = "";
        private const string ClientSecret = "";

        // 2. ID of your tenant
        private const string TenantId = "";

        static void Main(string[] args)
        {
        }

        private static async Task<string> GetTokenForClientAsync()
        {
            // NOTE: You cannot use the common endpoint when using app-only permission. We need your tenant ID.
            var authority = $"https://login.microsoftonline.com/{TenantId}/v2.0";

            var daemonClient = new ConfidentialClientApplication(
                ClientId,
                authority,
                ReplyUri,
                new ClientCredential(ClientSecret),
                null, null);

            // With app-only you cannot specify permission scopes on the fly. Only the default scope is accepted.
            string[] scopes = { "https://graph.microsoft.com/.default" };
            var result = await daemonClient.AcquireTokenForClientAsync(scopes);
            return result.AccessToken;
        }

        private static GraphServiceClient GetGraphClient()
        {
            var graphClient = new GraphServiceClient(
                "https://graph.microsoft.com/v1.0",
                new DelegateAuthenticationProvider(
                    async requestMessage =>
                    {
                        if (_token == null)
                        {
                            _token = await GetTokenForClientAsync();
                        }
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                    }));
            return graphClient;
        }
    }
}

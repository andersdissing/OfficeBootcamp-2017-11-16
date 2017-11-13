using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Spbg.ManageGroupsHol
{
    class Program
    {
        // Our very simple token cache
        private static string _token;

        // 1. Get the client id from your app registration on https://apps.dev.microsoft.com
        private const string ClientId = "";

        static void Main(string[] args)
        {
        }

        private static async Task<string> GetTokenForUserAsync()
        {
            var identityClientApp = new PublicClientApplication(ClientId);

            // You can add any permission scope you want here. The user will get prompted for consent the first time a new permission scope is added.
            string[] scopes = { "User.Read", "Group.ReadWrite.All" };
            AuthenticationResult authResult;
            try
            {
                // Look in cache for a token for this user with the specified scopes
                authResult = await identityClientApp.AcquireTokenSilentAsync(scopes, identityClientApp.Users.First());
                return authResult.AccessToken;
            }
            catch (Exception)
            {
                // Acquire a refresh and access token
                authResult = await identityClientApp.AcquireTokenAsync(scopes);
                return authResult.AccessToken;
            }
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
                            _token = await GetTokenForUserAsync();
                        }
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                    }));
            return graphClient;
        }
    }
}

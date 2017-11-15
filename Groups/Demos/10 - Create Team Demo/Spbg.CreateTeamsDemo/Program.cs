using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace Spbg.CreateTeamsDemo
{
    class Program
    {
        private static string _token;

        private const string ClientId = "";

        static void Main(string[] args)
        {
            // Get group to migrate
            var client = GetGraphClient();
            var groupFiltered = client.Groups
                .Request()
                .Filter("mailNickname eq 'group'")
                .GetAsync()
                .Result;
            var group = groupFiltered.First();

            // Create team
            try
            {
                using (var wc = new WebClient())
                {
                    wc.Headers["Authorization"] = $"Bearer {_token}";
                    wc.Headers["Content-Type"] = "application/json";
                    var responseJson = wc.UploadString($"https://graph.microsoft.com/beta/groups/{group.Id}/team", "PUT", "{}");
                    Console.WriteLine();
                    Console.WriteLine("Team created");
                    Console.WriteLine(responseJson);
                }
            }
            catch (WebException e)
            {
                using (var reader = new StreamReader(e.Response.GetResponseStream()))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
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
    }
}

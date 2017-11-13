using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Spbg.AuthorizationGrantDemo
{
    /// <summary>
    /// Simple example for Microsoft Graph with delegated permissions from a native application using the v2 endpoint in Azure AD.
    /// </summary>
    internal static class Program
    {
        private const string ClientId = "";

        static void Main(string[] args)
        {
            // Login
            var token = GetTokenForUserAsync().Result;
            Console.WriteLine("OAuth token");
            Console.WriteLine(token);

            // Get user profile
            using (var wc = new WebClient())
            {
                wc.Headers["Authorization"] = $"Bearer {token}";
                var responseJson = wc.DownloadString("https://graph.microsoft.com/v1.0/me/");
                Console.WriteLine();
                Console.WriteLine("User profile");
                Console.WriteLine(responseJson);
            }
        }

        private static async Task<string> GetTokenForUserAsync()
        {
            var identityClientApp = new PublicClientApplication(ClientId);

            // You can add any permission scope you want here. The user will get prompted for consent the first time a new permission scope is added.
            string[] scopes = { "User.Read" };
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

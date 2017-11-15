using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace Spbg.ClientCredentialsGrantDemo
{
    /// <summary>
    /// Simple example for a Microsoft Graph with app-only permission using the v2 endpoint in Azure AD.
    /// The code is based on the example shown here: 
    /// https://blogs.msdn.microsoft.com/tsmatsuz/2016/10/07/application-permission-with-v2-endpoint-and-microsoft-graph/
    /// </summary>
    internal class Program
    {
        // Get the values below from your app registration on https://apps.dev.microsoft.com
        private const string ClientId = "";
        private const string ReplyUri = "";
        private const string ClientSecret = "";

        // ID of your tenant
        private const string TenantId = "";

        static void Main(string[] args)
        {
            // Login
            var token = GetTokenForClientAsync().Result;

            Console.WriteLine("OAuth token");
            Console.WriteLine(token);

            // Get user profile by e-mail address
            using (var wc = new WebClient())
            {
                wc.Headers["Authorization"] = $"Bearer {token}";
                var responseJson = wc.DownloadString($"https://graph.microsoft.com/v1.0/users/admin@{TenantId}.onmicrosoft.com");

                Console.WriteLine();
                Console.WriteLine("User profile");
                Console.WriteLine(responseJson);
            }
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
    }
}

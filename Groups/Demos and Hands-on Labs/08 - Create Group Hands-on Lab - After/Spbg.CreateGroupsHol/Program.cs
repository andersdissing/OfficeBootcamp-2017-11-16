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
            // 3. Create Group without error handling
            //CreateGroup();

            // 4. Create Group with error handling
            //CreateGroupWithErrorHandling();

            // 5. Create group with SDK
            //CreateGroupWithSdk();

            // 6. Add 'user' to owner collection
            //Task.Run(() => AddOwnerToGroup("group", $"admin@{TenantId}"))
            //    .GetAwaiter()
            //    .GetResult();
        }

        private static async Task AddOwnerToGroup(string groupMailNickname, string ownerUpn)
        {
            var client = GetGraphClient();

            var userFiltered = await client.Users
                .Request()
                .Filter($"userPrincipalName eq '{ownerUpn}'")
                .GetAsync();
            var user = userFiltered.First();

            var groupFiltered = await client.Groups
                .Request()
                .Filter($"mailNickname eq '{groupMailNickname}'")
                .GetAsync();
            var group = groupFiltered.First();

            var owners = await client.Groups[group.Id].Owners
                .Request()
                .GetAsync();
            if (owners == null || owners.Count == 0)
            {
                // No owners yet. Add it
                await client.Groups[group.Id].Owners.References
                    .Request()
                    .AddAsync(user);
                Console.WriteLine($"Owner {user.UserPrincipalName} added to group {group.Mail}");
                return;
            }

            // Test if the user is already a member
            var ownersFiltered = await client.Groups[group.Id].Owners
                .Request()
                .Filter($"id eq '{user.Id}'")
                .GetAsync();
            if (ownersFiltered.Count == 0)
            {
                // Add 
                await client.Groups[group.Id].Owners.References
                    .Request()
                    .AddAsync(user);
                Console.WriteLine($"Owner {user.UserPrincipalName} added to group {group.Mail}");
            }
            else
            {
                Console.WriteLine("Owner already added");
            }
        }

        private static void CreateGroup()
        {
            var token = GetTokenForClientAsync().Result;

            using (var wc1 = new WebClient())
            {
                wc1.Headers["Authorization"] = $"Bearer {token}";
                wc1.Headers["Content-Type"] = "application/json";
                var body = @"{
                        'description': 'Group',
                        'displayName': 'Group',
                        'groupTypes': ['Unified'],
                        'mailEnabled': true,
                        'mailNickname': 'group',
                        'securityEnabled': false
                        }";
                var responseJson = wc1.UploadString("https://graph.microsoft.com/v1.0/groups", "POST", body);
                Console.WriteLine(responseJson);
            }
        }

        private static void CreateGroupWithErrorHandling()
        {
            var token = GetTokenForClientAsync().Result;

            try
            {
                var wc2 = new WebClient();
                wc2.Headers["Authorization"] = $"Bearer {token}";
                wc2.Headers["Content-Type"] = "application/json";
                var body = @"{
                'description': 'Group',
                'displayName': 'Group',
                'groupTypes': ['Unified'],
                'mailEnabled': true,
                'mailNickname': 'group',
                'securityEnabled': false
                }";
                var responseJson = wc2.UploadString("https://graph.microsoft.com/v1.0/groups", "POST", body);
                Console.WriteLine(responseJson);
            }
            catch (WebException e)
            {
                using (var reader = new StreamReader(e.Response.GetResponseStream()))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }

        private static void CreateGroupWithSdk()
        {
            var client = GetGraphClient();

            var group = new Group
            {
                Description = "Group",
                DisplayName = "Group",
                GroupTypes = new[] { "Unified" },
                MailEnabled = true,
                MailNickname = "group",
                SecurityEnabled = false
            };
            var result = client.Groups.Request().AddAsync(group).Result;
            Console.WriteLine(result.Mail);
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

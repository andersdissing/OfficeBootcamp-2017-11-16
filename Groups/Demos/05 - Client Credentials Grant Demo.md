# Client Credentials Grant Demo 05

## Steps
1. Open the Spbg.ClientCredentialsGrantDemo.sln in Visual Studio 2017
1. Insert your values for ClientId, ReplyUri, ClientSecret from MSAL application registration in Program.cs
1. Insert your TenantId in Program.cs - {tenant}.onmicrosoft.com
1. Insert the following snippet into Main method
	```csharp
    // Login
    var token = GetTokenForClientAsync().Result;

    Console.WriteLine("OAuth token");
    Console.WriteLine(token);

    // Get user profile by e-mail address
    using (var wc = new WebClient())
    {
        wc.Headers["Authorization"] = $"Bearer {token}";
        var responseJson = wc.DownloadString($"https://graph.microsoft.com/v1.0/users/admin@{TenantId}");

        Console.WriteLine();
        Console.WriteLine("User profile");
        Console.WriteLine(responseJson);
    }	
	```
1. Start the solution (CTRL-F5)
# Authorization Code Grant Demo 04

## Steps
1. Open the Spbg.AuthorizationGrantDemo.sln in Visual Studio 2017
1. Insert your ClientId in Program.cs
1. Add the following code to Main method
	```csharp
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
	```
1. Run the solution (CTRL-F5)
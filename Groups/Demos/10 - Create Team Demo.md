# Create Team Demo 10

## Steps
1. Start the Spbg.CreateTeamsDemo.sln in Visual Studio 2017
1. Check Authentication
	1. Authenticate with delegated permissions
	1. User must be a group owner
	1. Application must have Group.ReadWrite.All delegated permissions
1. Create the team (migrate group to team)
	1. Insert the ClientId
	1. Add the code below to Main method in Program.cs
	```csharp
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
	```
1. Check out documentation for more info - https://developer.microsoft.com/en-us/graph/docs/api-reference/beta/api/team_put_teams
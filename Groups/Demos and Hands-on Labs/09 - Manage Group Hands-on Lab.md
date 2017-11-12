# Microsoft Graph API Demo 01 - Docs

## Steps
1. Start with console application
	1. Locate 08 - Groups Hands-on Lab folder
	1. Open the Spbg.CreateGroupsHol.sln with Visual Studio 2017
1. Add Owner to a Office 365 Group
	1.Add this method to Progam.cs
		```csharp
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

			// Test if the user is already an owner
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
		```
	1. Then invoke it from Main method
		```csharp
		// 6. Add 'user' to owner collection
		Task.Run(() => AddOwnerToGroup("group6", $"bernd@{TenantId}"))
			.GetAwaiter()
			.GetResult();
		```
1. Close the solution in Visual Studio
1. Locate 09 - Manage Groups Hands-on Lab folder
1. Open the Spbg.ManageGroupsHol.sln with Visual Studio 2017
1. Open up Program.cs
	1. Fill-out the four constants in the top with the values for your Azure AD application and Office 365 tenant, for example:
		1. ClientId: 1329f001-1190-40cc-ac7b-710bd6f4ec01
	1. Add this code to the Main method
		```csharp
		var client = GetGraphClient();
		var group = client.Groups.Request().Filter("mailNickname eq 'group2'").GetAsync().Result.FirstOrDefault();

		var destination = new MemoryStream();
		using (var source = File.Open(@"logo.png", FileMode.Open))
		{
			source.CopyTo(destination);
		}
		destination.Position = 0;

		client.Groups[group.Id].Photo.Content.Request().PutAsync(destination).GetAwaiter().GetResult();	```csharp
		```

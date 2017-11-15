# Manage Groups Hands-on Lab - 09

## Steps
1. Locate '09 - Manage Groups Hands-on Lab' folder
1. Open the Spbg.ManageGroupsHol.sln with Visual Studio 2017
1. Open up Program.cs
	1. Fill-out the four constants in the top with the values for your Azure AD application and Office 365 tenant, for example:
		1. ClientId: 1329f001-1190-40cc-ac7b-710bd6f4ec01
	1. Add this code to the Main method
		```csharp
		var client = GetGraphClient();
		var group = client.Groups.Request().Filter("mailNickname eq 'group'").GetAsync().Result.FirstOrDefault();

		var destination = new MemoryStream();
		using (var source = File.Open(@"logo.png", FileMode.Open))
		{
			source.CopyTo(destination);
		}
		destination.Position = 0;

		client.Groups[group.Id].Photo.Content.Request().PutAsync(destination).GetAwaiter().GetResult();	
		```

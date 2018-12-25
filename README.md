# Bonkers
Simple REST API Client

## Example

```csharp
var client = new APIClient(new Uri("https://discordapp.com/api"));
var currentUser = client.CreateEndpoint("/users/@me").Header("Authorization", "token");

Console.WriteLine(await currentUser.Get());

var result = await currentUser.Get((req, res) =>
{
	Console.WriteLine($"Status: {res.StatusCode}");
	return res.Content.ReadAsStringAsync();
});

Console.WriteLine(await result);
```

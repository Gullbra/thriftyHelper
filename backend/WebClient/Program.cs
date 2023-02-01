using System.Net.Http.Headers;
using System.Reflection;
using WebAPIClient;


// creates our client - Now we've got a voice!
using HttpClient client = new();

// clears default(?)
client.DefaultRequestHeaders.Accept.Clear();

// set to accept Json responses (and github specific?)
client.DefaultRequestHeaders.Accept.Add(
		new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

// User-Agent header is necessary to retrieve info from github
client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

await HttpOperations.PrintReposName(client);
Console.WriteLine();


var repos = await HttpOperations.ReturnRepoObj(client);
foreach(var repo in repos)
{
	Console.WriteLine();
	PropertyInfo[] properties = typeof(Repository).GetProperties();
	foreach (PropertyInfo property in properties)
	{
		Console.WriteLine($"{property.Name}: " + property.GetValue(repo));
	}
}

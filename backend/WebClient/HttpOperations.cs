using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text.Json;

namespace WebAPIClient
{
	internal class HttpOperations
	{
		public static async Task PrintReposInfo(HttpClient client)
		{
			var json = await client.GetStringAsync(
				"https://api.github.com/users/gullbra/repos");

			Console.WriteLine(json);
		}

		public static async Task PrintReposName(HttpClient client)
		{
			await using Stream stream = await client.GetStreamAsync(
				"https://api.github.com/users/gullbra/repos");
			var repositories = await JsonSerializer.DeserializeAsync<List<Repository>>(stream);

			// coalescing operators (?? and ??=):
			//	for repo in (repositories or an empty list if repositories == null)
			foreach (var repo in repositories ?? Enumerable.Empty<Repository>())
				Console.WriteLine(repo.Name);
		}

		public static async Task<List<Repository>> ReturnRepoObj(HttpClient client)
		{
			await using Stream stream =
				await client.GetStreamAsync("https://api.github.com/users/gullbra/repos");

			var repositories =
					await JsonSerializer.DeserializeAsync<List<Repository>>(stream);

			return repositories ?? new();
		}
	}
}

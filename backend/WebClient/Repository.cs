using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

/*
	This is a class to repressent the incoming JSON object
	only the defined properties will be serialized into a C# object 
 */

namespace WebAPIClient
{
	// parsing the JSON properties into Repository Object Properties
	public record class Repository(
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("description")] string Description,
		[property: JsonPropertyName("fork")] bool Fork,
		[property: JsonPropertyName("pushed_at")] DateTime LastPushUtc)
	{
		public DateTime LastPush => LastPushUtc.ToLocalTime();
	}
}

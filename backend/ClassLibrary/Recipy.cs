using System.Linq;
using System.Text.Json.Serialization;

namespace ThriftyHelper.Backend.ClassLibrary;

public class Recipy
{
	public Recipy(int? id, string name, List<Ingredient>? ingredients, string description)
	{
		Id = id;
		Name = name.ToLower();
		Ingredients = ingredients;
		Description = description;
	}

	[JsonPropertyName("id")]
	public int? Id { get; }

	[JsonPropertyName("name")]
	public string Name { get; }

	[JsonPropertyName("ingredients")]
	public List<Ingredient>? Ingredients { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; }
}
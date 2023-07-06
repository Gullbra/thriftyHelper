using System.Linq;
using System.Text.Json.Serialization;

namespace ThriftyHelper.Backend.ClassLibrary;

public class Recipy
{
	public Recipy(int? id, string name, List<InredientReference>? ingredients, string description, List<string> inCategories)
	{
		Id = id;
		Name = name.ToLower();
		Ingredients = ingredients;
		Description = description;
		InCategories = inCategories;
	}

	[JsonPropertyName("id")]
	public int? Id { get; }

	[JsonPropertyName("name")]
	public string Name { get; }

	[JsonPropertyName("ingredients")]
	public List<InredientReference>? Ingredients { get; set; }

	[JsonPropertyName("description")]
	public string Description { get; }

	[JsonPropertyName("inCategories")]
	public List<string> InCategories { get; set; }
}


public class InredientReference
{
	public readonly int Id;
	public readonly double Quantity;
	public InredientReference(int id, double quantity)
	{
		Id = id;
		Quantity = quantity;
	}
}
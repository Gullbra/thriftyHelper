using System.Linq;

namespace ThriftyHelper.Backend.ClassLibrary;

public class Recipy
{
	public Recipy(string name, List<Ingredient> ingredients, string description)
	{
		Name = name.ToLower();
		Ingredients = ingredients;
		Description = description;
	}

	public string Name { get; }
	public List<Ingredient> Ingredients { get; }
	public string Description { get; }
}
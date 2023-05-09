using System.Linq;

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
	public int? Id { get; }
	public string Name { get; }
	public List<Ingredient>? Ingredients { get; set; }
	public string Description { get; }
}
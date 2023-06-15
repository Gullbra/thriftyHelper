using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.dbInit;


public class IngredientsData
{
	[JsonPropertyName("ingredientsList")]
	public List<IngredientFromJSON>? IngredientsList { get; set; }

	[JsonPropertyName("categories")]
	public List<string>? Categories { get; set; }
}

public class RecipiesData
{
	[JsonPropertyName("recipiesList")]
	public List<RecipyFromJSON>? RecipiesList { get; set; }

	[JsonPropertyName("categories")]
	public List<string>? Categories { get; set; }
}

public class TestData
{
	[JsonPropertyName("testArr")]
	public List<string>? TestArr { get; set; }
}


public record class IngredientFromJSON
(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("unit")] string Unit,
	[property: JsonPropertyName("pricePerUnit")] double PricePerUnit,
	[property: JsonPropertyName("energyPerUnit")] double EnergyPerUnit,
	[property: JsonPropertyName("proteinPerUnit")] double ProteinPerUnit,
	[property: JsonPropertyName("lastUpdated")] string LastUpdatedAsString,
	[property: JsonPropertyName("inCategories")] List<string> InCategories
)
{
	public DateTime LastUpdated => DateTime.Parse(LastUpdatedAsString);
}

public record class RecipyFromJSON
(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("name")] string Name,
	[property: JsonPropertyName("description")] string Description,
	[property: JsonPropertyName("ingredients")] List<RecipyIngredientList> Ingredients,
	[property: JsonPropertyName("inCategories")] List<string> InCategories
)
{ }

public record class RecipyIngredientList 
(
	[property: JsonPropertyName("id")] int Id,
	[property: JsonPropertyName("quantity")] int Quantity
)
{ }


public static class MockDataReader
{
	static readonly string basePath = String.Join("\\DbConnect\\", (System.Reflection.Assembly.GetEntryAssembly() ?? throw new Exception("Could not detirmine assembly path")).Location.Split("\\DbConnect\\").ToList().SkipLast(1)) + "\\DbConnect\\mock";
	static readonly string ingredientsDataPath = basePath + "\\ingredients.data.json";
	static readonly string recipiesDataPath = basePath + "\\recipies.data.json";
	static readonly string testDataPath = basePath + "\\test.json";

	private static async Task<T?> ReadAsync<T>(string filePath)
	{
		using FileStream stream = File.OpenRead(filePath);
		return await JsonSerializer.DeserializeAsync<T>(stream);
	}

	public static async Task<IngredientsData> GetIngredientsData() => await ReadAsync<IngredientsData>(ingredientsDataPath) ?? new IngredientsData();
	public static async Task<RecipiesData> GetRecipiesData() => await ReadAsync<RecipiesData>(recipiesDataPath) ?? new RecipiesData();
	public static async Task<TestData> GetTestArr() => await ReadAsync<TestData>(testDataPath) ?? new TestData();
	public static void CheckingPath() => Console.WriteLine($"\tPath test: {basePath}");
}
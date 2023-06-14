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
	public List<TestIng>? IngredientsList { get; set; }

	[JsonPropertyName("categories")]
	public List<string>? Categories { get; set; }
}

public class TestData
{
	//[property: JsonPropertyName("testArr")] List<string>? TestArr;
	[JsonPropertyName("testArr")]
	public List<string>? TestArr { get; set; }
}


public record class TestIng
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


public static class MockDataReader
{
	static readonly string ingredientsDataPath = Environment.CurrentDirectory + "\\mock\\ingredients.data.json";
	static readonly string recipiesDataPath = Environment.CurrentDirectory + "\\mock\\recipies.data.json";
	static readonly string testDataPath = "C:\\Users\\Martin\\Code\\repos\\thriftyHelper\\backend\\DbConnect\\mock\\test.json";

	public static async Task<IngredientsData> GetIngredientsData()
	{
		return await ReadAsync<IngredientsData>(ingredientsDataPath) ?? new IngredientsData();
	}

	public static async Task<TestData> GetTestArr()
	{
		var returnData = await ReadAsync<TestData>(testDataPath);
		Console.WriteLine($"returned: { returnData.TestArr }");
		return returnData;
	}


	public static async Task<T> ReadAsync<T>(string filePath)
	{
		using FileStream stream = File.OpenRead(filePath);
		var returnValue = await JsonSerializer.DeserializeAsync<T>(stream);
		Console.WriteLine("Deserialized!");
		return returnValue;
	}

	public static void Testing() => Console.WriteLine($"\ttesting:");
}
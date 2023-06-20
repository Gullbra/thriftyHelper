using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ThriftyHelper.Backend.ClassLibrary;

public class Ingredient
{
	public Ingredient(
		int id,
		string name, 
		string unit, 
		double pricePerUnit,
		double energyPerUnit, 
		double proteinPerUnit, 
		DateTime? dateTime,
		List<string> inCategories)

	{
		Id = id;
		Name = name.ToLower();
		Unit = unit.ToLower();
		PricePerUnit = pricePerUnit;
		EnergyPerUnit = energyPerUnit;
		ProteinPerUnit = proteinPerUnit;
		LastUpdated = dateTime;
		InCategories = inCategories;
	}
	[JsonPropertyName("id")]
	public int Id { get; set; }

	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("unit")]
	public string Unit { get; set; }

	[JsonPropertyName("pricePerUnit")]
	public double PricePerUnit { get; set; }

	[JsonPropertyName("energyPerUnit")]
	public double EnergyPerUnit { get; set; }

	[JsonPropertyName("proteinPerUnit")]
	public double ProteinPerUnit { get; set; }

	[JsonPropertyName("lastUpdated")]
	public DateTime? LastUpdated { get; set; }

	[JsonPropertyName("inCategories")]
	public List<string> InCategories { get; set; }
}


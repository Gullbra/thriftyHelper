using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

	public int Id { get; set; }
	public string Name { get; set; }
	public string Unit { get; set; }
	public double PricePerUnit { get; set; }
	public double EnergyPerUnit { get; set; }
	public double ProteinPerUnit { get; set; }
	public DateTime? LastUpdated { get; set; }
	public List<string> InCategories { get; set; }
}


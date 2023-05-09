using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftyHelper.Backend.ClassLibrary;
public class Ingredient
{
	public Ingredient(
		int? id, double? quantity, string name, string unit, 
		double pricePerUnit, double energyPerUnit, double proteinPerUnit, DateTime? dateTime)
	{
		Id = id;
		Name = name.ToLower();
		Quantity = quantity;
		Unit = unit.ToLower();
		PricePerUnit = pricePerUnit;
		EnergyPerUnit = energyPerUnit;
		ProteinPerUnit = proteinPerUnit;
		LastUpdated = dateTime;
	}

	public int? Id { get; set; }
	public double? Quantity { get; set; }
	public string Name { get; set; }
	public string Unit { get; set; }
	public double PricePerUnit { get; set; }
	public double EnergyPerUnit { get; set; }
	public double ProteinPerUnit { get; set; }
	public DateTime? LastUpdated { get; set; }
}

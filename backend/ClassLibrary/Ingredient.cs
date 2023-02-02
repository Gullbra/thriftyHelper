using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftyHelper.Backend.ClassLibrary;
public class Ingredient
{
	public Ingredient(
		string name, double quantity, string unit, 
		double pricePerUnit, double energyPerUnit, double proteinPerUnit)
	{
		Name = name.ToLower();
		Quantity = quantity;
		Unit = unit.ToLower();
		PricePerUnit = pricePerUnit;
		EnergyPerUnit = energyPerUnit;
		ProteinPerUnit = proteinPerUnit;
	}

	public string Name { get; set; }
	public double Quantity { get; set; }
	public string Unit { get; set; }
	public double PricePerUnit { get; set; }
	public double EnergyPerUnit { get; set; }
	public double ProteinPerUnit { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary;

public class IngredientInRecipyMap
{
	public IngredientInRecipyMap(int recipyId, int ingredientId, double quantity) { 
		RecipyId = recipyId;
		IngredientId = ingredientId;
		Quantity = quantity;
	}
	public int RecipyId { get; set; }
	public int IngredientId { get; set; }
	public double Quantity { get; set; }
}

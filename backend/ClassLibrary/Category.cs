using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary;

internal class Category
{
	Category(string? recipyId, string? ingredientId, string categoryId)
	{
		if(recipyId != null)
			RecipyId = recipyId;
		if ( ingredientId != null)
			IngredientId = ingredientId;

		CategoryId = categoryId;
	}

	public string? RecipyId { get; }
	public string? IngredientId { get; }
	public string CategoryId { get; }
}

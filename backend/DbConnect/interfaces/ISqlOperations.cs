using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.interfaces;

internal interface ISqlOperations
{
	List<Ingredient> GetIngredientsList();
	List<Recipy> GetRecipyList();

	Ingredient InsertNewIngredient(int id);
	Recipy InsertNewRecipy(int id);

	Ingredient UpdateIngredient(Ingredient ingredient);
	Recipy UpdateRecipy(Recipy recipy);

	Ingredient DeleteIngredient(int id);
	Recipy DeleteRecipy(int id);
}

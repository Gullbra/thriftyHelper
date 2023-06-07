using DbConnect.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.interfaces;

internal interface ISqlOperations
{
	Task<SqlResponseListIngredients> GetIngredientsList();
	//Task<List<Recipy>> GetRecipyList();

	Task<SqlResponseListCategories> GetCategoriesList(string type);

	//Task<SqlResponseSingleIngredient> InsertNewIngredient(int id);
	//Task<Recipy> InsertNewRecipy(int id);

	//Task<SqlResponseSingleIngredient> UpdateIngredient(Ingredient ingredient);
	//Task<Recipy> UpdateRecipy(Recipy recipy);

	//Task<SqlResponseSingleIngredient> DeleteIngredient(int id);
	//Task<Recipy> DeleteRecipy(int id);
}

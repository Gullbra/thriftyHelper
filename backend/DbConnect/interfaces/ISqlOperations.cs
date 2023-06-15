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
	Task<SqlResponse> GetIngredientsList();
	//Task<SqlResponse> GetRecipyList();

	//Task<SqlResponse> GetCategoriesList(string type);

	//Task<SqlResponse> InsertNewIngredient(int id);
	//Task<SqlResponse> InsertNewRecipy(int id);

	//Task<SqlResponse> UpdateIngredient(Ingredient ingredient);
	//Task<SqlResponse> UpdateRecipy(Recipy recipy);

	//Task<SqlResponse> DeleteIngredient(int id);
	//Task<SqlResponse> DeleteRecipy(int id);
}

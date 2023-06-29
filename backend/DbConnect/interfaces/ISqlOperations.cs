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
	public Task<SqlResponse<List<Ingredient>>> GetIngredientsList();
	public Task<SqlResponse<Ingredient>> InsertNewIngredient(Ingredient newIngredient);
	public Task<SqlResponse<Ingredient>> UpdateIngredient(Ingredient updatedIngredient, Ingredient currentIngredient);
	public Task<SqlResponse<Ingredient?>> DeleteIngredient(int ingredientId);





	//public Task<SqlResponse<List<Recipy>>> GetRecipyList();

	//public Task<SqlResponse> GetCategoriesList(string type);

	//public Task<SqlResponse> InsertNewRecipy(int id);

	//public Task<SqlResponse> UpdateRecipy(Recipy recipy);

	//public Task<SqlResponse> DeleteRecipy(int id);
}

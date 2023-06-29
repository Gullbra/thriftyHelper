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
	//public async Task<SqlResponse<Ingredient>> DeleteIngredient(int id);





	//public async Task<SqlResponse<List<Recipy>>> GetRecipyList();

	//public async Task<SqlResponse> GetCategoriesList(string type);

	//public async Task<SqlResponse> InsertNewRecipy(int id);

	//public async Task<SqlResponse> UpdateRecipy(Recipy recipy);

	//public async Task<SqlResponse> DeleteRecipy(int id);
}

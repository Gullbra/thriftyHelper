using DbConnect.Responses;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.interfaces;

public interface ISqlOperations
{
	public Task<SqlResponse<List<Ingredient>>> GetIngredientsList();
	public Task<SqlResponse<Ingredient>> InsertNewIngredient(Ingredient newIngredient);
	public Task<SqlResponse<Ingredient>> UpdateIngredient(Ingredient updatedIngredient, Ingredient currentIngredient);
	public Task<SqlResponse<Ingredient?>> DeleteIngredient(int ingredientId);





	public Task<SqlResponse<List<Recipy>>> GetRecipyList();

	//Task<SqlResponse> GetCategoriesList(string type);

	//Task<SqlResponse> InsertNewRecipy(int id);

	//Task<SqlResponse> UpdateRecipy(Recipy recipy);

	//Task<SqlResponse> DeleteRecipy(int id);
}

internal interface IIngredientsMethods
{
	Task<SqlResponse<List<Ingredient>>> GetIngredientsList();
	Task<SqlResponse<Ingredient>> InsertNewIngredient(Ingredient newIngredient);
	Task<SqlResponse<Ingredient>> UpdateIngredient(Ingredient updatedIngredient, Ingredient currentIngredient);
	Task<SqlResponse<Ingredient?>> DeleteIngredient(int ingredientId);
}

internal interface IRecipyMethods
{
	Task<SqlResponse<List<Recipy>>> GetRecipyList();
	Task<SqlResponse<Recipy>> GetOneRecipyById(int recipyId);
	Task<SqlResponse<Recipy>> UpdateRecipy(Recipy updatedRecipy, Recipy currentRecipy);
	Task<SqlResponse<Recipy>> DeleteRecipy(int recipyId);
}

internal interface ICategoriesMethods
{
	Task<SqlResponse<List<Category>>> GetAllCategoriesForItemType(string categoryType);
	Task<SqlResponse<Category>> InsertNewCategoryForItem(string categoryType, string newCategoryName, NpgsqlConnection conn);
	Task<SqlResponse<List<string>>> GetCategoriesForItem(string categoryType, int itemId);
}
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Responses;

public class SqlResponseListIngredients
{
	public SqlResponseListIngredients(bool success, List<Ingredient> data, string message)
	{
		Success = success;
		Data = data;
		Message = message;
	}

	public bool Success { get; }
	public List<Ingredient> Data { get; }
	public string Message { get; }
}
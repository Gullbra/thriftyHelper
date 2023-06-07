using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Responses;

public class SqlResponseSingleIngredient
{
	public SqlResponseSingleIngredient(bool success, Ingredient data, string message)
	{
		Success = success;
		Data = data;
		Message = message;
	}

	public bool Success { get; }
	public Ingredient Data { get; }
	public string Message { get; }
}

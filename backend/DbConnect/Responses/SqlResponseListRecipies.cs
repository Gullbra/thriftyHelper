using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Responses;

public class SqlResponseListRecipies
{
	public SqlResponseListRecipies(bool success, List<Recipy> data, string message)
	{
		Success = success;
		Data = data;
		Message = message;
	}

	public bool Success { get; }
	public List<Recipy> Data { get; }
	public string Message { get; }
}
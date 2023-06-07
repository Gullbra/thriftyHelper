using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Responses;

public class SqlResponseSingleRecipy
{
	public SqlResponseSingleRecipy(bool success, Recipy data, string message)
	{
		Success = success;
		Data = data;
		Message = message;
	}

	public bool Success { get; }
	public Recipy Data { get; }
	public string Message { get; }
}

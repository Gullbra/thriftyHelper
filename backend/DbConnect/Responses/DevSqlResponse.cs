namespace DbConnect.Responses;

public class DevSqlResponse
{
	public DevSqlResponse(bool success, string message)
	{
		Success = success;
		Message = message;
	}

	public bool Success { get; }
	public string Message { get; }
}

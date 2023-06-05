using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using DbConnect.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnect.interfaces;

public interface IDevSqlOperations
{
	public Task<DevSqlResponse> DevTestConnection();
	public Task<DevSqlResponse> DevSetUpTables();
	public Task<DevSqlResponse> DevTearDownTables();
	public Task<DevSqlResponse> DevReInitDb();
}

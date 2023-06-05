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
	public void DevSetUpTables();
	public void DevTearDownTables();
}

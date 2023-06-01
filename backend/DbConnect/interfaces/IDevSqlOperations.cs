using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnect.interfaces;

internal interface IDevSqlOperations
{
	public void DevTestConnection();
	public void DevSetUpTables();
	public void DevTearDownTables();
}


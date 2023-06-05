using DbConnect.interfaces;
using DbConnect.Responses;
using DbConnect.Sql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnect.SqlOperations;

public class DevSqlOperations : IDevSqlOperations
{
	private readonly NpgsqlDataSource dbDataSource;
	private readonly DevSql sqlStrings;

	public DevSqlOperations() : this(false) { }
	public DevSqlOperations(bool devMode)
	{
		sqlStrings = new DevSql(devMode);
		dbDataSource = NpgsqlDataSource.Create(ConnStr.Get(devMode));
	}

	public async Task<DevSqlResponse> DevTestConnection()
	{
		using var command = dbDataSource.CreateCommand(sqlStrings.DevTestConnection);
		await using var reader = await command.ExecuteReaderAsync();

		List<string> tablesInDb = new();
		while (await reader.ReadAsync())
		{
			tablesInDb.Add(reader.GetString(0));
		}

		return tablesInDb.Count == sqlStrings.TablesNamesList.Count
			? new DevSqlResponse(true, "All tables present!")
			: new DevSqlResponse(false, $"Missing tables: {String.Join(", ", sqlStrings.TablesNamesList.Where(tableName => !tablesInDb.Any(dbTable => dbTable == tableName)))}");
	}

	public void DevSetUpTables()
	{
		throw new NotImplementedException();
	}

	public void DevTearDownTables()
	{
		throw new NotImplementedException();
	}

}

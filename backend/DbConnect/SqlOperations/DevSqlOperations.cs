using DbConnect.interfaces;
using DbConnect.Responses;
using DbConnect.Sql;
using Npgsql;

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
		try
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
		catch (Exception ex)
		{
			return new DevSqlResponse(false, $"Error: {ex.Message}");
		}
	}

	public async Task<DevSqlResponse> DevSetUpTables()
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.SetUpTables);
			await using var reader = await command.ExecuteReaderAsync();

			return new DevSqlResponse(true, "Tables set upp!");
		}
		catch (Exception ex)
		{
			return new DevSqlResponse(false, $"Error: {ex.Message}");
		}
	}

	public async Task<DevSqlResponse> DevTearDownTables()
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.DevDropTables);
			await using var reader = await command.ExecuteReaderAsync();

			return new DevSqlResponse(true, "Tables dropped!");
		}
		catch (Exception ex)
		{
			return new DevSqlResponse(false, $"Error: {ex.Message}");
		}
	}
}

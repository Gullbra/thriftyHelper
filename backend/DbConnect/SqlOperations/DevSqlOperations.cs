using DbConnect.dbInit;
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

	public async Task<DevSqlResponse> DevReInitDb()
	{
		IngredientsData ingredientsData;
		RecipiesData recipiesData;

		try
		{
			ingredientsData = await MockDataReader.GetIngredientsData();
			recipiesData = await MockDataReader.GetRecipiesData();
		}
		catch (Exception ex)
		{
			return new DevSqlResponse(false, $"Error: {ex.Message}");
		}

		if (ingredientsData.IngredientsList == null || ingredientsData.Categories == null )
			return new DevSqlResponse(false, $"Error: no mock ingredients found in file");

		if (recipiesData.RecipiesList == null || recipiesData.Categories == null )
			return new DevSqlResponse(false, $"Error: no mock recipies found in file");


		var progressMessages = new List<string>();

		try
		{
			foreach (var category in ingredientsData.Categories)
			{
				Console.WriteLine(category);
			}
			progressMessages.Add("ingredient categories");

			foreach (var ingredient in ingredientsData.IngredientsList)
			{
				Console.WriteLine(ingredient.Name);       
			}
			progressMessages.Add("ingredients");

			foreach (var category in recipiesData.Categories)
			{
				Console.WriteLine(category);
			}
			progressMessages.Add("recipy categories");

			foreach (var recipy in recipiesData.RecipiesList)
			{
				Console.WriteLine(recipy.Name);
			}
			progressMessages.Add("recipies");

			return new DevSqlResponse(true, $"Created entries:\n{
				ingredientsData.Categories.Count} ingredient categories\n{
				ingredientsData.IngredientsList.Count} ingredients\n{
				recipiesData.Categories.Count} recipy categories\n{
				recipiesData.RecipiesList.Count} recipies");

		} catch (Exception ex)
		{
			var listOperationsNotCompleted = new List<string>() { 
				"ingredient categories",
				"ingredients",
				"recipy categories",
				"recipies"
			}.Except(progressMessages);
			return new DevSqlResponse(false, $"Error: {ex.Message} {String.Join(", ", listOperationsNotCompleted)} NOT (fully) inserted");
		}
	}
}

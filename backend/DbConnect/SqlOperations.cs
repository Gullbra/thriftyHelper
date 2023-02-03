using Npgsql;
using System.Data.Common;
using System.Linq;
using ThriftyHelper.Backend.ClassLibrary;

namespace ThriftyHelper.Backend.DbConnect;
public class SqlOperations
{
	internal static NpgsqlConnection OpenConnection()
	{
		string connectionString = $"Host={Sectrets.Host};Username={Sectrets.Username};Password={Sectrets.Password};Database={Sectrets.Database}";
		return new NpgsqlConnection(connectionString);
	}

	public static Recipy GetRecipy(string recipyName)
	{
		NpgsqlConnection dbConnection = OpenConnection();
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new(
			"SELECT * " +
			"FROM stored_recipies " +
			$"WHERE name = '{recipyName}';",
			dbConnection);
		NpgsqlDataReader reader = sqlCommand.ExecuteReader();
		reader.Read();
		var rec_id = reader.GetInt32(0);
		var rec_desc = reader.GetString(2);

		Console.WriteLine("\n\tfirst query:");
		Console.WriteLine($"recipy_id: {rec_id}");
		Console.WriteLine($"recipy_desc: {rec_desc}");


		dbConnection.Close(); dbConnection.Open();
		sqlCommand = new(
			"SELECT * " +
			"FROM ingredients_in_recepies " +
			$"WHERE recipy_id = {rec_id};",
			dbConnection);
		reader = sqlCommand.ExecuteReader();

		List<int> ingredientsIdList = new ();
		List<double> ingredientsQuantityList = new();
		Console.WriteLine("\n\tsecond query:");
		while (reader.Read())
		{
			ingredientsIdList.Add(reader.GetInt32(1));
			ingredientsQuantityList.Add(reader.GetDouble(2));
			Console.WriteLine($"Ingredient_id: {reader.GetInt32(1)}");
			Console.WriteLine($"Quantity: {reader.GetDouble(2)}");
		}


		dbConnection.Close(); dbConnection.Open();
		sqlCommand = new(
			"SELECT * " +
			"FROM stored_ingredients " +
			"WHERE " + string.Join(" OR ", ingredientsIdList.Select(num => $"ingredient_id = {num}")) + ";",
			dbConnection);
		reader = sqlCommand.ExecuteReader();

		List<Ingredient> ingredientsList = new ();
		int i = 0;
		while (reader.Read())
		{
			ingredientsList.Add(new Ingredient(
				reader.GetString(1),
				ingredientsQuantityList[i],
				reader.GetString(2),
				reader.GetDouble(3),
				reader.GetDouble(4),
				reader.GetDouble(5)));
			i++;
		}

		Console.WriteLine("\n\tthird query:");
		foreach (Ingredient ingredient in ingredientsList)
		{
			Console.WriteLine($"name: {ingredient.Name}, unit: {ingredient.Unit}, Price/unit: {ingredient.PricePerUnit}, E/unit: {ingredient.EnergyPerUnit}, P/unit: {ingredient.ProteinPerUnit}");
		}


		dbConnection.Close();
		return new Recipy (recipyName, ingredientsList, rec_desc);
	}
}

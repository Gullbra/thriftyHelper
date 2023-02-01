
//using ThriftyHelper.Backend.ClassLibrary;

using Npgsql;
using System.Data.Common;
using System.Linq;
using ThriftyHelper.Backend.DbConnect;

namespace Backend.DbConnect;
internal class SqlOperations
{
	internal static NpgsqlConnection OpenConnection()
	{
		Sectrets ENV = new();

		string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
		return new NpgsqlConnection(connectionString);

	}
	public static void test()
	{
		/*
		Sectrets ENV = new();

		string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
		using NpgsqlConnection dbConnection = new(connectionString);
		*/
		NpgsqlConnection dbConnection = OpenConnection();
		dbConnection.Open();

		string recipyName = "blodpudding med ägg och bacon";

		NpgsqlCommand sqlCommand = new(
			"SELECT * " +
			"FROM stored_recipies " +
			$"WHERE name = '{recipyName}';",
			dbConnection);

		NpgsqlDataReader reader = sqlCommand.ExecuteReader();

		reader.Read();
		var rec_id = reader.GetInt32(0);
		var rec_desc = reader.GetString(2);

		Console.WriteLine("\tfirst query:");
		Console.WriteLine($"recipy_id: {rec_id}");
		Console.WriteLine($"recipy_desc: {rec_desc}");
		Console.WriteLine();

		dbConnection.Close();
		dbConnection = OpenConnection();
		dbConnection.Open();

		sqlCommand = new(
			"SELECT * " +
			"FROM ingredients_in_recepies " +
			$"WHERE recipy_id = {rec_id};",
			dbConnection);

		reader = sqlCommand.ExecuteReader();

		List<int> ingredients_id_list = new ();
		List<double> ingredients_quantity_list = new();
		while (reader.Read())
		{
			ingredients_id_list.Add(reader.GetInt32(1));
			Console.WriteLine($"Ingredient_id: {reader.GetInt32(1)}");
			ingredients_quantity_list.Add(reader.GetDouble(2));
			Console.WriteLine($"Quantity: {reader.GetDouble(2)}");
		}

		dbConnection.Close();
		dbConnection = OpenConnection();
		dbConnection.Open();

		//string tesingWhereQuery = "WHERE " + string.Join(", ", ingredients_id_list.Select(num => $"ingredient_id = {num}"));
		sqlCommand = new(
			"SELECT * " +
			"FROM stored_ingredients " +
			"WHERE " + string.Join(" OR ", ingredients_id_list.Select(num => $"ingredient_id = {num}")) + ";",
			dbConnection);

		reader = sqlCommand.ExecuteReader();

		//List<string> ingredients_name_list = new();
		//List<string> ingredients_quantity_list = new();
		while (reader.Read())
		{
			Console.WriteLine($"Ingredient name: {reader.GetString(1)}");
			Console.WriteLine($"Ingredient unit: {reader.GetString(2)}");
			Console.WriteLine($"Price/unit: {reader.GetDouble(3)}");
			Console.WriteLine($"E/unit: {reader.GetDouble(4)}");
			Console.WriteLine($"Protein/unit: {reader.GetDouble(5)}");
		}
		/*

		*/
	}
}

using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DbConnect;
internal class SqlOperations
{
	public static string GetSqlVersion(NpgsqlConnection dbConnection)
	{
		/* Access Scalar */
		string sqlString = "SELECT version()";
		using NpgsqlCommand sqlCommand = new (sqlString, dbConnection);
 
		string? sqlVersion = sqlCommand.ExecuteScalar().ToString();
		return $"PostgreSQL version: {sqlVersion}";
	}

	public static void CreateTable(NpgsqlConnection dbConnection) 
	{
		/* Create Table*/
		using NpgsqlCommand sqlCommand = new();
		sqlCommand.Connection = dbConnection;

		sqlCommand.CommandText = "DROP TABLE IF EXISTS cars";
		sqlCommand.ExecuteNonQuery();

		/* SERIAL makes the column auto--increment in POstgreSQL*/
		sqlCommand.CommandText = @"CREATE TABLE cars(id SERIAL PRIMARY KEY, name VARCHAR(255), price INT)";
		sqlCommand.ExecuteNonQuery();

		List<KeyValuePair<string, int>> carList = new()
		{
			new KeyValuePair<string, int>("Audi", 52642),
			new KeyValuePair<string, int>("Mercedes", 57127),
			new KeyValuePair<string, int>("Skoda", 9000),
			new KeyValuePair<string, int>("Volvo", 29000),
			new KeyValuePair<string, int>("Bently",350000),
			new KeyValuePair<string, int>("Citroën", 21000),
			new KeyValuePair<string, int>("Volkswagen", 21600)
		};

		foreach (var kv in carList)
		{
			sqlCommand.CommandText = $"INSERT INTO cars(name, price) VALUES ('{kv.Key}', {kv.Value})";
			sqlCommand.ExecuteNonQuery();
		}

		Console.WriteLine("Table cars created");
	}

	public static void InsertRow(NpgsqlConnection dbConnection, string name = "BMW", int price = 36600)
	{
		using NpgsqlCommand sqlCommand = new(); sqlCommand.Connection = dbConnection;
		/* @ infront of values adds a placeholder parameter, which protects against SQL injection attacks*/
		sqlCommand.CommandText = "INSERT INTO cars(name, price) VALUES(@name, @price);";

		/* actual values added as parameters*/
		sqlCommand.Parameters.AddWithValue("name", name);
		sqlCommand.Parameters.AddWithValue("price", price);

		/* command built */
		sqlCommand.Prepare();

		sqlCommand.ExecuteNonQuery();
		Console.WriteLine("Row inserted");
	}

	public static void GetAllColumns(NpgsqlConnection dbConnection)
	{
		using NpgsqlCommand sqlCommand = new();
		sqlCommand.Connection = dbConnection;
		sqlCommand.CommandText = "SELECT * FROM cars;"; /* in this case equal to: "SELECT id, name, price FROM cars;"*/

		/* NpgsqlDataReader: a fast, forward-only and read-only object to access query results */
		using NpgsqlDataReader reader= sqlCommand.ExecuteReader();

		/* reading results one row at a time */
		while (reader.Read()) 
		{
			Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2));
		}
	}

	public static void GetAllWithColumnHeaders (NpgsqlConnection dbConnection)
	{
		using NpgsqlCommand sqlCommand = new("SELECT * FROM cars", dbConnection);

		using NpgsqlDataReader reader = sqlCommand.ExecuteReader();

		Console.WriteLine($"{reader.GetName(0),-4} {reader.GetName(1),-10} {reader.GetName(2),10}");

		while (reader.Read())
		{
			Console.WriteLine($"{reader.GetInt32(0),-4} {reader.GetString(1),-10} {reader.GetInt32(2),10}");
		}
	}
}

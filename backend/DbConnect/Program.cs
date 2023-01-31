using Npgsql;
using System;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.DbConnect;

Sectrets ENV = new();

string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
using NpgsqlConnection dbConnection = new(connectionString);

dbConnection.Open();

/* Access Scalar
string sqlString = "SELECT version()";
using NpgsqlCommand sqlCommand = new (sqlString, dbConnection);
 
string sqlVersion = sqlCommand.ExecuteScalar().ToString();
Console.WriteLine($"PostgreSQL version: {sqlVersion}");
 */

/* Create Table*/
using NpgsqlCommand sqlCommand = new();
sqlCommand.Connection = dbConnection;

sqlCommand.CommandText = "DROP TABLE IF EXISTS cars";
sqlCommand.ExecuteNonQuery();

sqlCommand.CommandText = @"CREATE TABLE cars(id SERIAL PRIMARY KEY,
	name VARCHAR(255), price INT)";
sqlCommand.ExecuteNonQuery();

List<KeyValuePair<string, int>> carList = new ()
{
	new KeyValuePair<string, int>("Audi", 52642),
	new KeyValuePair<string, int>("Mercedes", 57127),
	new KeyValuePair<string, int>("Skoda", 9000),
	new KeyValuePair<string, int>("Volve", 29000),
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
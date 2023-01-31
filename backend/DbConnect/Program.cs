using Npgsql;
using System;
using ThriftyHelper.Backend.DbConnect;

Sectrets ENV = new();

string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
using NpgsqlConnection dbConnection = new(connectionString);

dbConnection.Open();

string sqlString = "SELECT version()";
using NpgsqlCommand sqlCommand = new (sqlString, dbConnection);

string sqlVersion = sqlCommand.ExecuteScalar().ToString();
Console.WriteLine($"PostgreSQL version: {sqlVersion}");
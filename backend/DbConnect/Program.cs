using DbConnect;
using Npgsql;
using System;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.DbConnect;

/*
 https://zetcode.com/csharp/postgresql/
 */

Sectrets ENV = new();

string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
using NpgsqlConnection dbConnection = new(connectionString);

dbConnection.Open();

SqlOperations.GetSqlVersion(dbConnection);

// SqlOperations.CreateTable(dbConnection);

// SqlOperations.InsertRow(dbConnection);

// SqlOperations.GetAllColumns(dbConnection);

SqlOperations.GetAllWithColumnHeaders(dbConnection);

dbConnection.Close();
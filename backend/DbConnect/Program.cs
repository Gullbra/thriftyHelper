using Npgsql;
using System;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.DbConnect;
using ThriftyHelper.Backend.ClassLibrary;
using DbConnect.SqlOperations;
using DbConnect.dbInit;

/*
 https://zetcode.com/csharp/postgresql/
 */

//var Test = new DevSqlOperations(true);
//var responesTear = await Test.DevTearDownTables(); Console.WriteLine($"Tables dropped: {responesTear.Success}");
//var resposeBuild = await Test.DevSetUpTables(); Console.WriteLine($"Tables build: {resposeBuild.Success}");
//var response = await Test.DevReInitDb();
//Console.WriteLine($"Success: {response.Success}, Message: {response.Message}");

var TestSqlOps = new SqlOperations(true);
var ingredientList = await TestSqlOps.GetIngredientsList();
if (ingredientList.Success)
{
	if (ingredientList.DataIngredientList != null && ingredientList.DataIngredientList.Count > 0)
	{
		foreach (var cat in ingredientList.DataIngredientList[0].InCategories)
		{
			Console.WriteLine(cat);
		}
	}
}
else
{
	Console.WriteLine(ingredientList.Message);
}


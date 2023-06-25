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

var Test = new DevSqlOperations(true);
var responesTear = await Test.DevTearDownTables(); Console.WriteLine($"Tables dropped: {responesTear.Success}");
var resposeBuild = await Test.DevSetUpTables(); Console.WriteLine($"Tables build: {resposeBuild.Success}");
var response = await Test.DevReInitDb();
Console.WriteLine($"Success: {response.Success}, Message: {response.Message}");

var TestSqlOps = new SqlOperations(true);

var InsertRespone = await TestSqlOps.InsertNewIngredient(new Ingredient(
	id: null,
	name: "testing insert", 
	unit: "g", 
	pricePerUnit: 20.0, 
	energyPerUnit: 20.0, 
	proteinPerUnit: 20.0,
	dateTime: null,
	inCategories: new List<string>() { "testingredients"}
));

Console.WriteLine($"succes: {InsertRespone.Success} - Message: {InsertRespone.Message}");


//var ingredientList = await TestSqlOps.GetIngredientsList();
//if (ingredientList.Success)
//{
//	if (ingredientList.Data != null && ingredientList.Data.Count > 0)
//	{
//		foreach (var cat in ingredientList.Data[0].InCategories)
//		{
//			Console.WriteLine(cat);
//		}
//	}
//}
//else
//{
//	Console.WriteLine(ingredientList.Message);
//}


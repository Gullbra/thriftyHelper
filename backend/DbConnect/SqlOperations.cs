using Npgsql;
using System.Data.Common;
using System.Linq;
using System.Reflection.PortableExecutable;
using ThriftyHelper.Backend.ClassLibrary;

namespace ThriftyHelper.Backend.DbConnect;

public class ConnStr
{
	public static string Get(bool dbLocal)
	{
		if (dbLocal)
			return string.Format(
				"Host={0};Username={1};Password={2};Database={3}", 
				Sectrets.Host, 
				Sectrets.Username, 
				Sectrets.Password, 
				Sectrets.Database);

		var uri = new Uri(Sectrets.HostedConnectionString);
		return string.Format(
			"Server={0};Database={1};User Id={2};Password={3};Port={4}",
			uri.Host, 
			uri.AbsolutePath.Trim('/'), 
			uri.UserInfo.Split(':')[0], 
			uri.UserInfo.Split(':')[1], 
			uri.Port > 0 ? uri.Port : 5432);

		//var uriString = Sectrets.HostedConnectionString;
		//var uri = new Uri(uriString);
		//var db = uri.AbsolutePath.Trim('/');
		//var user = uri.UserInfo.Split(':')[0];
		//var passwd = uri.UserInfo.Split(':')[1];
		//var port = uri.Port > 0 ? uri.Port : 5432;
		//var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
		//		uri.Host, db, user, passwd, port);
		//return connStr;
	}
}

public class SqlOperations
{
	static NpgsqlConnection dbConnection;
	static bool dbLocal;
	static string recipyTableName;
	static string ingredientsTableName;
	static string ingredientsInRecipiesTableName;

	public SqlOperations() : this(false) { }
	public SqlOperations(bool devMode)
	{
		dbLocal = devMode;
		dbConnection = new NpgsqlConnection(ConnStr.Get(devMode));
		recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
		ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
		ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";
	}

	public void TestingConnection()
	{
		Console.WriteLine($"\tTesting: Select: Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new(
			"SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'; ",
			dbConnection);

		try {
			var reader = sqlCommand.ExecuteReader();
			List<string> recipyNames = new();
			while (reader.Read())
			{
				recipyNames.Add(reader.GetString(0));
				Console.WriteLine($"Table: {reader.GetString(0)}");
			}
		}
		catch (Exception err) { Console.WriteLine($"Excepton encountered: {err.Message}"); }

		dbConnection.Close();
	}

	public void SetUpDb()
	{
		Console.WriteLine($"\tSetting up db: db: Hosted={!dbLocal}:");

		dbConnection.Open();
		NpgsqlCommand sqlCommand = new($@"
			CREATE TABLE IF NOT EXISTS {recipyTableName} (
				recipy_id       SERIAL        PRIMARY KEY,
				recipy_name     varchar(120)  NOT NULL,
				description     text          NOT NULL,
				UNIQUE (recipy_id, recipy_name)
			);

			CREATE TABLE IF NOT EXISTS {ingredientsTableName} (
				ingredient_id   	SERIAL        PRIMARY KEY,
				ingredient_name   varchar(255)  NOT NULL,
				ingredient_unit		varchar(20)   NOT NULL,
				price_per_unit		float(8)			NOT NULL,
				energy_per_unit		float(8)			NOT NULL,
				protein_per_unit	float(8)			NOT NULL,
				UNIQUE (ingredient_id, ingredient_name)
			);

			CREATE TABLE IF NOT EXISTS {ingredientsInRecipiesTableName} (
				recipy_id 				integer 			REFERENCES stored_recipies ON DELETE RESTRICT,
				ingredient_id			integer 			REFERENCES stored_ingredients ON DELETE RESTRICT,
				PRIMARY KEY(recipy_id, ingredient_id),
				quantity 					float(4)			NOT NULL	
			);",
			dbConnection);

		try
		{
			sqlCommand.ExecuteNonQuery();
			Console.WriteLine("Tables set up!");
		}
		catch (Exception err) { Console.WriteLine($"Tables NOT created successfully: {err.Message}"); }

		dbConnection.Close();
	}

	public void InsertNewIngredient(Ingredient newIngredient)
	{
		Console.WriteLine($"Inserting new Ingredient: db: Hosted={!dbLocal}: \n");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new(
			$@"INSERT INTO {ingredientsTableName} (
				ingredient_name
				ingredient_unit
				price_per_unit
				energy_per_unit
				protein_per_unit			
			) VALUES (
				'{newIngredient.Name}'
				'{newIngredient.Unit}'
				{newIngredient.PricePerUnit}
				{newIngredient.EnergyPerUnit}
				{newIngredient.ProteinPerUnit}
			) RETURN *;",
			dbConnection);

		try 
		{ 
			var reader = sqlCommand.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"Ingredient_id: {reader.GetInt32(0)}, Ingredient_name: {reader.GetString(1)}");
			}
		}
		catch (Exception err) { Console.WriteLine($"Eception Encountered: {err.Message}"); }

		dbConnection.Close();
	}
}

/*

INSERT INTO ingredients_in_recepies (
	recipy_id, 
	ingredient_id,
	quantity
) VALUES (
	(SELECT recipy_id FROM stored_recipies WHERE name='blodpudding med ägg och bacon'),
	(SELECT ingredient_id FROM stored_ingredients WHERE name='bacon'),
	70
); 

*/



public class OldSqlOperations
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



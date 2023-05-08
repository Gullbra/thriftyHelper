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
	}
}

public class SqlOperations
{
	private readonly NpgsqlConnection dbConnection;
	// private readonly NpgsqlDataSource dbDataSource;
	private readonly bool dbLocal;
	private readonly string recipyTableName;
	private readonly string ingredientsTableName;
	private readonly string ingredientsInRecipiesTableName;

	public SqlOperations() : this(false) { }
	public SqlOperations(bool devMode)
	{
		dbLocal = devMode;
		dbConnection = new NpgsqlConnection(ConnStr.Get(devMode));
		// dbDataSource = NpgsqlDataSource.Create(ConnStr.Get(devMode));
		recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
		ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
		ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";
	}

	public void TestConnection()
	{
		Console.WriteLine($"\n\tTesting Connection: Hosted={!dbLocal}:");
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
		catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }

		dbConnection.Close();
	}

	public void SetUpDb()
	{
		Console.WriteLine($"\n\tSetting up db: Hosted={!dbLocal}:");

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

	/* Ingredients*/
	public Ingredient? GetIngredientByName(string ingredientName) 
	{
		Console.WriteLine($"\n\tRetrieving ingredient \"{ingredientName}\": Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new($@"
			SELECT * 
			FROM {ingredientsTableName}  
			WHERE ingredient_name = '{ingredientName}';",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			Console.WriteLine("SQL success!");

			while (reader.Read())
			{
				return new Ingredient(
					reader.GetInt32(0),
					null,
					reader.GetString(1),
					reader.GetString(2),
					reader.GetDouble(3),
					reader.GetDouble(4),
					reader.GetDouble(5),
					reader.GetDateTime(6));
			}
		}
		catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
		finally { dbConnection.Close(); }
		return null;
	}
	public List<Ingredient>? GetIngredientsList() 
	{
		Console.WriteLine($"\n\tRetrieving all stored ingredients: Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new($@"
			SELECT * 
			FROM {ingredientsTableName};",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			List<Ingredient> ingredientsList = new();
			while (reader.Read())
			{
				ingredientsList.Add(new Ingredient(
					reader.GetInt32(0),
					null,
					reader.GetString(1),
					reader.GetString(2),
					reader.GetDouble(3),
					reader.GetDouble(4),
					reader.GetDouble(5),
					reader.GetDateTime(6)));
			}
			Console.WriteLine("SQL success!");
			return ingredientsList;
		}
		catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
		finally { dbConnection.Close(); }
		return null;
	}

	public Ingredient? InsertNewIngredient(Ingredient newIngredient)
	{
		Console.WriteLine($"\n\tInserting new Ingredient {newIngredient.Name}: Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new(
			$@"INSERT INTO {ingredientsTableName} (
				ingredient_name,
				ingredient_unit,
				price_per_unit,
				energy_per_unit,
				protein_per_unit,
				last_updated
			) VALUES (
				'{newIngredient.Name}',
				'{newIngredient.Unit}',
				{newIngredient.PricePerUnit},
				{newIngredient.EnergyPerUnit},
				{newIngredient.ProteinPerUnit},
				CURRENT_TIMESTAMP
			) RETURNING *;",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"Ingredient_id: {reader.GetInt32(0)},\nIngredient_name: {reader.GetString(1)},\nIngredient_unit: {reader.GetString(2)},\nPrice_per_unit: {reader.GetDouble(3)},\nEnergy_per_price: {reader.GetDouble(4)},\nProtein_per_price: {reader.GetDouble(5)}");

				return new Ingredient(
					reader.GetInt32(0),
					null,
					reader.GetString(1),
					reader.GetString(2),
					reader.GetDouble(3),
					reader.GetDouble(4),
					reader.GetDouble(5),
					reader.GetDateTime(6));
			}
		}
		catch (Exception err) { Console.WriteLine($"Eception Encountered: {err.Message}"); }
		finally { dbConnection.Close(); }

		return null;
	}
	public Ingredient? UpdateIngredient(Ingredient updateInfo)
	{
		Console.WriteLine($"\n\tUpdating Ingredient {updateInfo.Name}: Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new($@"
			UPDATE {ingredientsTableName}
			SET 
				ingredient_name = '{updateInfo.Name}',
				price_per_unit = {updateInfo.PricePerUnit},
				energy_per_unit = {updateInfo.EnergyPerUnit},
				protein_per_unit = {updateInfo.ProteinPerUnit},
				last_updated = CURRENT_TIMESTAMP
			WHERE ingredient_id = {updateInfo.Id}
			RETURNING *;",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			while (reader.Read())
			{
				Console.WriteLine($"Ingredient_id: {reader.GetInt32(0)},\nIngredient_name: {reader.GetString(1)},\nIngredient_unit: {reader.GetString(2)},\nPrice_per_unit: {reader.GetDouble(3)},\nEnergy_per_price: {reader.GetDouble(4)},\nProtein_per_price: {reader.GetDouble(5)}");

				return new Ingredient(
					reader.GetInt32(0),
					null,
					reader.GetString(1),
					reader.GetString(2),
					reader.GetDouble(3),
					reader.GetDouble(4),
					reader.GetDouble(5),
					reader.GetDateTime(6));
			}
		}
		catch (Exception err) { Console.WriteLine($"Eception Encountered: {err.Message}"); }
		finally { dbConnection.Close(); }

		return null;
	}

	/* Recipies*/
	public Recipy? GetRecipyByName(string recipyName)
	{
		Console.WriteLine($"\n\tRetrieving recipy \"{recipyName}\": Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new($@"
			SELECT st.*, ip.*, si.*
			FROM {recipyTableName} st
				INNER JOIN {ingredientsInRecipiesTableName} ip
					ON st.recipy_id = ip.recipy_id
				INNER JOIN {ingredientsTableName} si
					ON ip.ingredient_id = si.ingredient_id
			WHERE recipy_name = '{recipyName}';
			",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			Console.WriteLine("SQL success!");

			Recipy? returnRecipy = null;
			List<Ingredient> ingredientsList = new();

			while (reader.Read())
			{
				returnRecipy ??= new Recipy(
					reader.GetInt32(0),
					reader.GetString(1),
					null,
					reader.GetString(2));

				ingredientsList.Add(new Ingredient(
					reader.GetInt32(4),
					reader.GetDouble(5),
					reader.GetString(7),
					reader.GetString(8),
					reader.GetDouble(9),
					reader.GetDouble(10),
					reader.GetDouble(11),
					reader.GetDateTime(12)));
			}

			returnRecipy.Ingredients = ingredientsList;

			return returnRecipy;
		}
		catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
		finally { dbConnection.Close(); }

		return null;
	}
	public List<Recipy>? GetRecipyList()
	{
		Console.WriteLine($"\n\tRetrieving all stored recipies: Hosted={!dbLocal}:");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new($@"
			SELECT * 
			FROM {recipyTableName};",
			dbConnection);

		try
		{
			var reader = sqlCommand.ExecuteReader();
			List<Recipy> recipyList = new();
			while (reader.Read())
			{
				recipyList.Add(new Recipy(
					reader.GetInt32(0),
					reader.GetString(1),
					new List<Ingredient>(),
					reader.GetString(2)
				));
			}
			Console.WriteLine("SQL success!");
			return recipyList;
		}
		catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
		finally { dbConnection.Close(); }
		return null;
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
					reader.GetInt32(0),
					null,
					reader.GetString(1),
					reader.GetString(2),
					reader.GetDouble(3),
					reader.GetDouble(4),
					reader.GetDouble(5),
					reader.GetDateTime(6)));
			i++;
		}

		Console.WriteLine("\n\tthird query:");
		foreach (Ingredient ingredient in ingredientsList)
		{
			Console.WriteLine($"name: {ingredient.Name}, unit: {ingredient.Unit}, Price/unit: {ingredient.PricePerUnit}, E/unit: {ingredient.EnergyPerUnit}, P/unit: {ingredient.ProteinPerUnit}");
		}


		dbConnection.Close();
		return new Recipy (null, recipyName, ingredientsList, rec_desc);
	}
}



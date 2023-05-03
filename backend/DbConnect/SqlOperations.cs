using Npgsql;
using System.Data.Common;
using System.Linq;
using System.Reflection.PortableExecutable;
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


public class ConnStrHosted
{
	public static string Get()
	{
		var uriString = Sectrets.HostedConnectionString;
		var uri = new Uri(uriString);
		var db = uri.AbsolutePath.Trim('/');
		var user = uri.UserInfo.Split(':')[0];
		var passwd = uri.UserInfo.Split(':')[1];
		var port = uri.Port > 0 ? uri.Port : 5432;
		var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
				uri.Host, db, user, passwd, port);
		return connStr;
	}
}


public class NewSqlOperations
{
	static NpgsqlConnection dbConnection;
	static bool dbLocal;

	public NewSqlOperations() : this(true) { }
	public NewSqlOperations (bool devMode)
	{
		dbConnection = new NpgsqlConnection(devMode 
			? $"Host={Sectrets.Host};Username={Sectrets.Username};Password={Sectrets.Password};Database={Sectrets.Database}"
			: ConnStrHosted.Get());

		dbLocal = devMode;
	}

	public void TestingConnection()
	{
		Console.WriteLine($"Testing: Select: Hosted={!dbLocal}: \n");
		dbConnection.Open();

		NpgsqlCommand sqlCommand = new(
			dbLocal
				? "SELECT name FROM stored_recipies;"
				: "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'; ",
			dbConnection);
		var reader = sqlCommand.ExecuteReader();


		List<string> recipyNames = new();
		while (reader.Read())
		{			
			recipyNames.Add(reader.GetString(0));
			Console.WriteLine($"Recipy: {reader.GetString(0)}");
		}

		dbConnection.Close();
	}

	public void SetUpDb ()
	{
		Console.WriteLine($"Setting up db: db: Hosted={!dbLocal}: \n");

		dbConnection.Open();
		//try { } catch ()
		NpgsqlCommand sqlCommand = new(@"
			CREATE TABLE IF NOT EXISTS thrifty_helper__stored_recipies (
				recipy_id       SERIAL        PRIMARY KEY,
				recipy_name     varchar(120)  NOT NULL,
				description     text          NOT NULL
			);

			CREATE TABLE IF NOT EXISTS thrifty_helper__stored_ingredients (
				ingredient_id   	SERIAL          PRIMARY KEY,
				ingredient_name   varchar(255)  NOT NULL,
				ingredient_unit		varchar(20)   NOT NULL,
				price_per_unit		float(8)			NOT NULL,
				energy_per_unit		float(8)			NOT NULL,
				protein_per_unit	float(8)			NOT NULL
			);

			CREATE TABLE IF NOT EXISTS thrifty_helper__stored_recipies (
				recipy_id 				integer 			REFERENCES stored_recipies,
				ingredient_id			integer 			REFERENCES stored_ingredients,
				PRIMARY KEY(recipy_id, ingredient_id),
				quantity 					float(4)			NOT NULL	
			);");
		dbConnection.Close();

	}

	public void InsertNewIngredient(Ingredient newIngredient)
	{
		Console.WriteLine($"Inserting new Ingredient: db: Hosted={!dbLocal}: \n");
		//dbConnection.Open();

		//NpgsqlCommand sqlCommand = new(
		//	dbLocal
		//		? "INSERT INTO thrifty_helper__stored_recipies;"
		//		: "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'; ",
		//	dbConnection);
		//var reader = sqlCommand.ExecuteReader();


		//List<string> recipyNames = new();
		//while (reader.Read())
		//{
		//	recipyNames.Add(reader.GetString(0));
		//	Console.WriteLine($"Recipy: {reader.GetString(0)}");
		//}

		//dbConnection.Close();
	}
}
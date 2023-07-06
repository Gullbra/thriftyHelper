
using DbConnect.interfaces;
using DbConnect.Responses;
using Npgsql;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.SqlOperations;
public class SqlOperations : ISqlOperations
{
	private readonly NpgsqlDataSource dbDataSource;
	private readonly Sql.Sql sqlStrings;

	public SqlOperations() : this(false) { }
	public SqlOperations(bool devMode)
	{
		sqlStrings = new Sql.Sql(devMode);
		dbDataSource = NpgsqlDataSource.Create(ConnStr.Get(devMode));
	}

	public async Task<SqlResponse<List<Ingredient>>> GetIngredientsList()
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.GetIngredients);
			await using var reader = await command.ExecuteReaderAsync();

			List<Ingredient> ingredientsList = new();
			while (await reader.ReadAsync())
			{
				var categoriesResponse = await GetCategoriesForItem("ingredient", reader.GetInt32(0));

				if (!categoriesResponse.Success || categoriesResponse.Data == null)
					return new SqlResponse<List<Ingredient>>(false, new List<Ingredient>(), categoriesResponse.Message);

				ingredientsList.Add(new Ingredient(
					id: reader.GetInt32(0),
					name: reader.GetString(1),
					unit: reader.GetString(2),
					pricePerUnit: reader.GetDouble(3),
					energyPerUnit: reader.GetDouble(4),
					proteinPerUnit: reader.GetDouble(5),
					dateTime: reader.GetDateTime(6),
					inCategories: categoriesResponse.Data));
			}

			return new SqlResponse<List<Ingredient>>(true, ingredientsList, $"Success!");
		}
		catch(Exception ex)
		{
			return new SqlResponse<List<Ingredient>>(false, new List<Ingredient>(), $"Error: ${ex.Message}");
		}
	}

	public async Task<SqlResponse<Ingredient?>> GetOneIngredientById(int ingredientId)
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.GetIngredientById);
			command.Parameters.AddWithValue("@i_id", ingredientId);
			await using var reader = await command.ExecuteReaderAsync();

			List<Ingredient> ingredientsList = new();
			while (await reader.ReadAsync())
			{
				var categoriesResponse = await GetCategoriesForItem("ingredient", ingredientId);

				if (!categoriesResponse.Success || categoriesResponse.Data == null)
					return new SqlResponse<Ingredient?>(false, null, categoriesResponse.Message);

				ingredientsList.Add(new Ingredient(
					id: reader.GetInt32(0),
					name: reader.GetString(1),
					unit: reader.GetString(2),
					pricePerUnit: reader.GetDouble(3),
					energyPerUnit: reader.GetDouble(4),
					proteinPerUnit: reader.GetDouble(5),
					dateTime: reader.GetDateTime(6),
					inCategories: categoriesResponse.Data));
			}

			return new SqlResponse<Ingredient?>(true, ingredientsList.Count == 0 ? null : ingredientsList[0], $"Success!");
		}
		catch (Exception ex)
		{
			return new SqlResponse<Ingredient?>(false, null, $"Error: ${ex.Message}");
		}
	}

	public async Task<SqlResponse<Ingredient>> InsertNewIngredient(Ingredient newIngredient)
	{
		/*
		Insert new category if category doesn't exist -> return cat_Id
		Insert ingredient -> return ing_Id
			insert ingredient-category mapping
	 */
		var conn = await dbDataSource.OpenConnectionAsync();
		var categoryList = new List<Category>();

		foreach(var category in newIngredient.InCategories)
		{
			var response = await InsertNewCategoryForItem("ingredient", category, conn);

			if (!response.Success)
			{
				conn.Close();
				return new SqlResponse<Ingredient>(false, newIngredient, "new category data insertion fail:" + response.Message);
			}
			categoryList.Add(response.Data);
		}

		try
		{
			await using var cmd = new NpgsqlCommand(sqlStrings.InsertNewIngredient, conn);
			cmd.Parameters.AddWithValue("@i_n", newIngredient.Name);
			cmd.Parameters.AddWithValue("@i_u", newIngredient.Unit);
			cmd.Parameters.AddWithValue("@prPU", newIngredient.PricePerUnit);
			cmd.Parameters.AddWithValue("@ePU", newIngredient.EnergyPerUnit);
			cmd.Parameters.AddWithValue("@pPU", newIngredient.ProteinPerUnit);
			using var reader = await cmd.ExecuteReaderAsync();

			while (reader.Read())
			{
				newIngredient.Id = reader.GetInt32(0);
				newIngredient.LastUpdated = reader.GetDateTime(6);
			}
		}
		catch (Exception ex)
		{
			conn.Close();
			return new SqlResponse<Ingredient>(false, newIngredient, "Ingredient data insertion fail:" + ex.Message);
		}

		try
		{
			if (newIngredient.Id == null)
			{
				conn.Close();
				return new SqlResponse<Ingredient>(false, newIngredient, "No ingredient id returned after insertion");
			}

			foreach(var cate in categoryList)
			{
				if (cate.CategoryId == null)
				{ conn.Close(); return new SqlResponse<Ingredient>(false, newIngredient, "Missing category Id"); }

				await using var cmd = new NpgsqlCommand(sqlStrings.InsertIngredientCategoryMapping, conn);
				cmd.Parameters.AddWithValue("@i_c_id", cate.CategoryId);
				cmd.Parameters.AddWithValue("@i_id", newIngredient.Id);
				cmd.ExecuteNonQuery();
			}
		}
		catch (Exception ex)
		{
			conn.Close();
			return new SqlResponse<Ingredient>(false, newIngredient, "Ingredient-category mapping fail:" + ex.Message);
		}

		conn.Close();
		return new SqlResponse<Ingredient>(true, newIngredient, "suxcces");
	}

	public async Task<SqlResponse<Ingredient>> UpdateIngredient(Ingredient updatedIngredient, Ingredient currentIngredient)
	{
		/*
			check delta (old and new data)
				
			if (change in cat)
			{
				(Upsert instead?)
				get categories from names => category_id (ON CONFLICT SKIP?)
					if (retrieved cat.count != inCategories.count)
						foreach (category.notInDb)
							insert new categories
								=> return category_id

				(delete all old mappings then add new) 
			}

			Update ingredient table

		 */


		if (updatedIngredient.Id == null || currentIngredient.Id == null)
			return new SqlResponse<Ingredient>(false, updatedIngredient, "No id provided");

		if (updatedIngredient.Id != currentIngredient.Id)
			return new SqlResponse<Ingredient>(false, updatedIngredient, "provedid ingredients data does not have matching id's");

		using var conn = await dbDataSource.OpenConnectionAsync();

		if (updatedIngredient.InCategories.Count != currentIngredient.InCategories.Count || !updatedIngredient.InCategories.All(currentIngredient.InCategories.Contains))
		{
			var response = await GetAllCategoriesForItemType("ingredient");

			if (!response.Success)
			{
				conn.Close();
				return new SqlResponse<Ingredient>(false, updatedIngredient, response.Message);
			}

			/* Insert new category if needed */
			var newCategories = updatedIngredient.InCategories.Except(response.Data.Select(cat => cat.CategoryName).ToList());
			if (newCategories.Count() > 0) 
			{
				List<Category> relevantCategories = response.Data.Where(cat => updatedIngredient.InCategories.Contains(cat.CategoryName)).ToList();

				foreach (var cat in newCategories)
				{
					var response2 = await InsertNewCategoryForItem("ingredient", cat, conn);

					if (!response2.Success)
					{
						conn.Close();
						return new SqlResponse<Ingredient>(false, updatedIngredient, response2.Message);
					}

					relevantCategories.Add(response2.Data);
				}

				// category-ingredient mapping
				await using var cmd = new NpgsqlCommand(sqlStrings.DeleteOldIngredintCategoryMappings, conn);
				cmd.Parameters.AddWithValue("@i_id", updatedIngredient.Id);
				await cmd.ExecuteNonQueryAsync();

				foreach(var category in relevantCategories)
				{
					await using var cmd2 = new NpgsqlCommand(sqlStrings.InsertIngredientCategoryMapping, conn);
					cmd2.Parameters.AddWithValue("@i_id", updatedIngredient.Id);
					cmd2.Parameters.AddWithValue("@i_c_id", category.CategoryId);
					await cmd2.ExecuteNonQueryAsync();
				}
			}
		}

		try
		{
			await using var cmd = new NpgsqlCommand(sqlStrings.UpdateIngredient, conn);
			cmd.Parameters.AddWithValue("@i_id", updatedIngredient.Id);
			cmd.Parameters.AddWithValue("@i_n", updatedIngredient.Name);
			cmd.Parameters.AddWithValue("@i_u", updatedIngredient.Unit);
			cmd.Parameters.AddWithValue("@prPU", updatedIngredient.PricePerUnit);
			cmd.Parameters.AddWithValue("@ePU", updatedIngredient.EnergyPerUnit);
			cmd.Parameters.AddWithValue("@pPU", updatedIngredient.ProteinPerUnit);
			using var reader = await cmd.ExecuteReaderAsync();

			while (reader.Read())
			{
				updatedIngredient.LastUpdated = reader.GetDateTime(6);
			}
		}
		catch (Exception ex)
		{
			conn.Close();
			return new SqlResponse<Ingredient>(false, updatedIngredient, "Ingredient data insertion fail:" + ex.Message);
		}

		return new SqlResponse<Ingredient>(true, updatedIngredient, "Success!");
	}

	public async Task<SqlResponse<Ingredient?>> DeleteIngredient(int ingredientId)
	{
		try
		{
			using var conn = await dbDataSource.OpenConnectionAsync();

			var categoriesResponse = await GetCategoriesForItem("ingredient", ingredientId);

			if (!categoriesResponse.Success || categoriesResponse.Data == null)
				throw new Exception($"error:" + categoriesResponse.Message);

			await using var cmdC = new NpgsqlCommand(sqlStrings.DeleteOldIngredintCategoryMappings, conn);
			cmdC.Parameters.AddWithValue("@i_id", ingredientId);
			await cmdC.ExecuteNonQueryAsync();

			await using var cmd = new NpgsqlCommand(sqlStrings.DeleteIngredient, conn);
			cmd.Parameters.AddWithValue("@i_id", ingredientId);
			using var reader = await cmd.ExecuteReaderAsync();

			Ingredient? deletedIngredient = null;
			while (reader.Read())
			{
				deletedIngredient = new Ingredient(
					id: reader.GetInt32(0),
					name: reader.GetString(1),
					unit: reader.GetString(2),
					pricePerUnit: reader.GetDouble(3),
					energyPerUnit: reader.GetDouble(4),
					proteinPerUnit: reader.GetDouble(5),
					dateTime: reader.GetDateTime(6),
					inCategories: categoriesResponse.Data);
			}
			return new SqlResponse<Ingredient?>(true, deletedIngredient ?? null, "Success");
		}
		catch(Exception ex)
		{
			return new SqlResponse<Ingredient?>(false, null, ex.Message);
		}
	}

	public async Task<SqlResponse<List<Recipy>>> GetRecipyList()
	{
		throw new NotImplementedException();
	}


	/* Helper mehtods*/
	private async Task<SqlResponse<List<Category>>> GetAllCategoriesForItemType(string categoryType)
	{
		if (categoryType != "recipy" && categoryType != "ingredient")
			return new SqlResponse<List<Category>>(false, new(), "Trying to retrieve categories of non-existant category types");

		try
		{
			using var command = dbDataSource.CreateCommand(categoryType != "ingredient" ? sqlStrings.GetAllIngredientCategories : sqlStrings.GetAllRecipyCategories);
			await using var reader = await command.ExecuteReaderAsync();

			List<Category> categoriesList = new();
			while (await reader.ReadAsync())
			{
				categoriesList.Add(new Category(
					categoryId: reader.GetInt32(0),
					categoryName: reader.GetString(1)
				));
			}

			return new SqlResponse<List<Category>>(true, categoriesList, $"Success!");
		}
		catch (Exception ex)
		{
			return new SqlResponse<List<Category>>(false, new(), $"Error: ${ex.Message}");
		}
	}

	private async Task<SqlResponse<Category>> InsertNewCategoryForItem(string categoryType, string newCategoryName, NpgsqlConnection conn)
	{
		if (categoryType != "recipy" && categoryType != "ingredient")
			return new SqlResponse<Category>(false, new(), "Trying to insert non-existant category types");
		try
		{
			await using var cmd = new NpgsqlCommand
			(
				categoryType == "ingredient" 
					? sqlStrings.InsertNewIngredientCategory
					: sqlStrings.InsertNewRecipyCategory, 
				conn
			);
			cmd.Parameters.AddWithValue("@c_n", newCategoryName);
			await using var reader = await cmd.ExecuteReaderAsync();
			var returnedCategory = new Category();
			while (reader.Read())
			{
				returnedCategory = new(reader.GetString(1), reader.GetInt32(0));
			}
			return new SqlResponse<Category>(true, returnedCategory, "success");
		}
		catch(Exception ex)
		{
			return new SqlResponse<Category>(false, new(), message: ex.Message);
		}
	}

	private async Task<SqlResponse<List<string>>> GetCategoriesForItem(string categoryType, int itemId)
	{
		if (categoryType != "recipy" && categoryType != "ingredient")
			return new SqlResponse<List<string>>(false, new List<string>(), "Trying to retrieve non-existant category types");

		using var conn = await dbDataSource.OpenConnectionAsync();

		try
		{
			using var cmd = new NpgsqlCommand
			(
				categoryType == "ingredient"
					? sqlStrings.GetCategoriesFromIngredientId
					: sqlStrings.GetCategoriesFromRecipyId,
				conn
			);
			cmd.Parameters.AddWithValue("@id", itemId);

			using var reader = cmd.ExecuteReader();

			List<string> categories = new();
			while (await reader.ReadAsync())
			{
				categories.Add(reader.GetString(0));
			}
			return new SqlResponse<List<string>>(true, categories, $"Categories for ingredient {itemId} retrieved");
		}
		catch(Exception ex)
		{
			return new SqlResponse<List<string>>(false, new List<string>(), ex.Message);
		}
		finally
		{
			conn.Close();
		}
	}

}


internal class IngredientMethods : IIngredientsMethods 
{ 
}




//public class SqlOperations
//{
//    private readonly NpgsqlConnection dbConnection;
//    // private readonly NpgsqlDataSource dbDataSource;
//    private readonly bool dbLocal;
//    private readonly string recipyTableName;
//    private readonly string ingredientsTableName;
//    private readonly string ingredientsInRecipiesTableName;

//    public SqlOperations() : this(false) { }
//    public SqlOperations(bool devMode)
//    {
//        dbLocal = devMode;
//        dbConnection = new NpgsqlConnection(ConnStr.Get(devMode));
//        // dbDataSource = NpgsqlDataSource.Create(ConnStr.Get(devMode));
//        recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
//        ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
//        ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";
//    }

//    public void TestConnection()
//    {
//        Console.WriteLine($"\n\tTesting Connection: Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new(
//            "SELECT tablename FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'; ",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            List<string> recipyNames = new();
//            while (reader.Read())
//            {
//                recipyNames.Add(reader.GetString(0));
//                Console.WriteLine($"Table: {reader.GetString(0)}");
//            }
//        }
//        catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }

//        dbConnection.Close();
//    }

//    public void SetUpDb()
//    {
//        Console.WriteLine($"\n\tSetting up db: Hosted={!dbLocal}:");

//        dbConnection.Open();
//        NpgsqlCommand sqlCommand = new($@"
//			CREATE TABLE IF NOT EXISTS {recipyTableName} (
//				recipy_id       SERIAL        PRIMARY KEY,
//				recipy_name     varchar(120)  NOT NULL,
//				description     text          NOT NULL,
//				UNIQUE (recipy_id, recipy_name)
//			);

//			CREATE TABLE IF NOT EXISTS {ingredientsTableName} (
//				ingredient_id   	SERIAL        PRIMARY KEY,
//				ingredient_name   varchar(255)  NOT NULL,
//				ingredient_unit		varchar(20)   NOT NULL,
//				price_per_unit		float(8)			NOT NULL,
//				energy_per_unit		float(8)			NOT NULL,
//				protein_per_unit	float(8)			NOT NULL,
//				UNIQUE (ingredient_id, ingredient_name)
//			);

//			CREATE TABLE IF NOT EXISTS {ingredientsInRecipiesTableName} (
//				recipy_id 				integer 			REFERENCES stored_recipies ON DELETE RESTRICT,
//				ingredient_id			integer 			REFERENCES stored_ingredients ON DELETE RESTRICT,
//				PRIMARY KEY(recipy_id, ingredient_id),
//				quantity 					float(4)			NOT NULL	
//			);",
//            dbConnection);

//        try
//        {
//            sqlCommand.ExecuteNonQuery();
//            Console.WriteLine("Tables set up!");
//        }
//        catch (Exception err) { Console.WriteLine($"Tables NOT created successfully: {err.Message}"); }

//        dbConnection.Close();
//    }

//    /* Ingredients*/
//    public Ingredient? GetIngredientByName(string ingredientName)
//    {
//        Console.WriteLine($"\n\tRetrieving ingredient \"{ingredientName}\": Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new($@"
//			SELECT * 
//			FROM {ingredientsTableName}  
//			WHERE ingredient_name = '{ingredientName}';",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            Console.WriteLine("SQL success!");

//            while (reader.Read())
//            {
//                return new Ingredient(
//                    reader.GetInt32(0),
//                    null,
//                    reader.GetString(1),
//                    reader.GetString(2),
//                    reader.GetDouble(3),
//                    reader.GetDouble(4),
//                    reader.GetDouble(5),
//                    reader.GetDateTime(6));
//            }
//        }
//        catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }
//        return null;
//    }
//    public List<Ingredient>? GetIngredientsList()
//    {
//        Console.WriteLine($"\n\tRetrieving all stored ingredients: Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new($@"
//			SELECT * 
//			FROM {ingredientsTableName};",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            List<Ingredient> ingredientsList = new();
//            while (reader.Read())
//            {
//                ingredientsList.Add(new Ingredient(
//                    reader.GetInt32(0),
//                    null,
//                    reader.GetString(1),
//                    reader.GetString(2),
//                    reader.GetDouble(3),
//                    reader.GetDouble(4),
//                    reader.GetDouble(5),
//                    reader.GetDateTime(6)));
//            }
//            Console.WriteLine("SQL success!");
//            return ingredientsList;
//        }
//        catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }
//        return null;
//    }

//    public Ingredient? InsertNewIngredient(Ingredient newIngredient)
//    {
//        Console.WriteLine($"\n\tInserting new Ingredient {newIngredient.Name}: Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new(
//            $@"INSERT INTO {ingredientsTableName} (
//				ingredient_name,
//				ingredient_unit,
//				price_per_unit,
//				energy_per_unit,
//				protein_per_unit,
//				last_updated
//			) VALUES (
//				'{newIngredient.Name}',
//				'{newIngredient.Unit}',
//				{newIngredient.PricePerUnit},
//				{newIngredient.EnergyPerUnit},
//				{newIngredient.ProteinPerUnit},
//				CURRENT_TIMESTAMP
//			) RETURNING *;",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            while (reader.Read())
//            {
//                Console.WriteLine($"Ingredient_id: {reader.GetInt32(0)},\nIngredient_name: {reader.GetString(1)},\nIngredient_unit: {reader.GetString(2)},\nPrice_per_unit: {reader.GetDouble(3)},\nEnergy_per_price: {reader.GetDouble(4)},\nProtein_per_price: {reader.GetDouble(5)}");

//                return new Ingredient(
//                    reader.GetInt32(0),
//                    null,
//                    reader.GetString(1),
//                    reader.GetString(2),
//                    reader.GetDouble(3),
//                    reader.GetDouble(4),
//                    reader.GetDouble(5),
//                    reader.GetDateTime(6));
//            }
//        }
//        catch (Exception err) { Console.WriteLine($"Eception Encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }

//        return null;
//    }
//    public Ingredient? UpdateIngredient(Ingredient updateInfo)
//    {
//        Console.WriteLine($"\n\tUpdating Ingredient {updateInfo.Name}: Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new($@"
//			UPDATE {ingredientsTableName}
//			SET 
//				ingredient_name = '{updateInfo.Name}',
//				price_per_unit = {updateInfo.PricePerUnit},
//				energy_per_unit = {updateInfo.EnergyPerUnit},
//				protein_per_unit = {updateInfo.ProteinPerUnit},
//				last_updated = CURRENT_TIMESTAMP
//			WHERE ingredient_id = {updateInfo.Id}
//			RETURNING *;",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            while (reader.Read())
//            {
//                Console.WriteLine($"Ingredient_id: {reader.GetInt32(0)},\nIngredient_name: {reader.GetString(1)},\nIngredient_unit: {reader.GetString(2)},\nPrice_per_unit: {reader.GetDouble(3)},\nEnergy_per_price: {reader.GetDouble(4)},\nProtein_per_price: {reader.GetDouble(5)}");

//                return new Ingredient(
//                    reader.GetInt32(0),
//                    null,
//                    reader.GetString(1),
//                    reader.GetString(2),
//                    reader.GetDouble(3),
//                    reader.GetDouble(4),
//                    reader.GetDouble(5),
//                    reader.GetDateTime(6));
//            }
//        }
//        catch (Exception err) { Console.WriteLine($"Eception Encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }

//        return null;
//    }

//    /* Recipies*/
//    public Recipy? GetRecipyByName(string recipyName)
//    {
//        Console.WriteLine($"\n\tRetrieving recipy \"{recipyName}\": Hosted={!dbLocal}:");
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new($@"
//			SELECT st.*, ip.*, si.*
//			FROM {recipyTableName} st
//				INNER JOIN {ingredientsInRecipiesTableName} ip
//					ON st.recipy_id = ip.recipy_id
//				INNER JOIN {ingredientsTableName} si
//					ON ip.ingredient_id = si.ingredient_id
//			WHERE recipy_name = '{recipyName}';
//			",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            Console.WriteLine("SQL success!");

//            Recipy? returnRecipy = null;
//            List<Ingredient> ingredientsList = new();

//            while (reader.Read())
//            {
//                returnRecipy ??= new Recipy(
//                    reader.GetInt32(0),
//                    reader.GetString(1),
//                    null,
//                    reader.GetString(2));

//                ingredientsList.Add(new Ingredient(
//                    reader.GetInt32(4),
//                    reader.GetDouble(5),
//                    reader.GetString(7),
//                    reader.GetString(8),
//                    reader.GetDouble(9),
//                    reader.GetDouble(10),
//                    reader.GetDouble(11),
//                    reader.GetDateTime(12)));
//            }

//            returnRecipy.Ingredients = ingredientsList;

//            return returnRecipy;
//        }
//        catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }

//        return null;
//    }
//    public List<Recipy>? GetRecipyList()
//    {
//        Console.WriteLine($"\n\tRetrieving all stored recipies: Hosted={!dbLocal}:");
//        dbConnection.Open();

//        //NpgsqlCommand sqlCommand = new($@"
//        //	SELECT * 
//        //	FROM {recipyTableName};",
//        //	dbConnection);

//        NpgsqlCommand sqlCommand = new($@"
//			SELECT st.*, ip.*, si.*
//			FROM {recipyTableName} st
//				INNER JOIN {ingredientsInRecipiesTableName} ip
//					ON st.recipy_id = ip.recipy_id
//				INNER JOIN {ingredientsTableName} si
//					ON ip.ingredient_id = si.ingredient_id;
//			",
//            dbConnection);

//        try
//        {
//            var reader = sqlCommand.ExecuteReader();
//            List<Recipy> recipyList = new();
//            while (reader.Read())
//            {
//                if (recipyList.Count == 0 || recipyList.Last().Id != reader.GetInt32(0))
//                {
//                    recipyList.Add(new Recipy(
//                        reader.GetInt32(0),
//                        reader.GetString(1),
//                        new List<Ingredient>()
//                        { new Ingredient(
//                                reader.GetInt32(4),
//                                reader.GetDouble(5),
//                                reader.GetString(7),
//                                reader.GetString(8),
//                                reader.GetDouble(9),
//                                reader.GetDouble(10),
//                                reader.GetDouble(11),
//                                reader.GetDateTime(12))
//                        },
//                        reader.GetString(2)));
//                }
//                else
//                {
//                    recipyList.Last().Ingredients.Add(new Ingredient(
//                        reader.GetInt32(4),
//                        reader.GetDouble(5),
//                        reader.GetString(7),
//                        reader.GetString(8),
//                        reader.GetDouble(9),
//                        reader.GetDouble(10),
//                        reader.GetDouble(11),
//                        reader.GetDateTime(12)));
//                }
//            }
//            Console.WriteLine("SQL success!");
//            return recipyList;
//        }
//        catch (Exception err) { Console.WriteLine($"Exception encountered: {err.Message}"); }
//        finally { dbConnection.Close(); }
//        return null;
//    }
//}


///*

//INSERT INTO ingredients_in_recepies (
//	recipy_id, 
//	ingredient_id,
//	quantity
//) VALUES (
//	(SELECT recipy_id FROM stored_recipies WHERE name='blodpudding med ägg och bacon'),
//	(SELECT ingredient_id FROM stored_ingredients WHERE name='bacon'),
//	70
//); 

//*/



//public class OldSqlOperations
//{
//    internal static NpgsqlConnection OpenConnection()
//    {
//        string connectionString = $"Host={Sectrets.Host};Username={Sectrets.Username};Password={Sectrets.Password};Database={Sectrets.Database}";
//        return new NpgsqlConnection(connectionString);
//    }

//    public static Recipy GetRecipy(string recipyName)
//    {
//        NpgsqlConnection dbConnection = OpenConnection();
//        dbConnection.Open();

//        NpgsqlCommand sqlCommand = new(
//            "SELECT * " +
//            "FROM stored_recipies " +
//            $"WHERE name = '{recipyName}';",
//            dbConnection);
//        NpgsqlDataReader reader = sqlCommand.ExecuteReader();
//        reader.Read();
//        var rec_id = reader.GetInt32(0);
//        var rec_desc = reader.GetString(2);

//        Console.WriteLine("\n\tfirst query:");
//        Console.WriteLine($"recipy_id: {rec_id}");
//        Console.WriteLine($"recipy_desc: {rec_desc}");


//        dbConnection.Close(); dbConnection.Open();
//        sqlCommand = new(
//            "SELECT * " +
//            "FROM ingredients_in_recepies " +
//            $"WHERE recipy_id = {rec_id};",
//            dbConnection);
//        reader = sqlCommand.ExecuteReader();

//        List<int> ingredientsIdList = new();
//        List<double> ingredientsQuantityList = new();
//        Console.WriteLine("\n\tsecond query:");
//        while (reader.Read())
//        {
//            ingredientsIdList.Add(reader.GetInt32(1));
//            ingredientsQuantityList.Add(reader.GetDouble(2));
//            Console.WriteLine($"Ingredient_id: {reader.GetInt32(1)}");
//            Console.WriteLine($"Quantity: {reader.GetDouble(2)}");
//        }


//        dbConnection.Close(); dbConnection.Open();
//        sqlCommand = new(
//            "SELECT * " +
//            "FROM stored_ingredients " +
//            "WHERE " + string.Join(" OR ", ingredientsIdList.Select(num => $"ingredient_id = {num}")) + ";",
//            dbConnection);
//        reader = sqlCommand.ExecuteReader();

//        List<Ingredient> ingredientsList = new();
//        int i = 0;
//        while (reader.Read())
//        {
//            ingredientsList.Add(new Ingredient(
//                    reader.GetInt32(0),
//                    null,
//                    reader.GetString(1),
//                    reader.GetString(2),
//                    reader.GetDouble(3),
//                    reader.GetDouble(4),
//                    reader.GetDouble(5),
//                    reader.GetDateTime(6)));
//            i++;
//        }

//        Console.WriteLine("\n\tthird query:");
//        foreach (Ingredient ingredient in ingredientsList)
//        {
//            Console.WriteLine($"name: {ingredient.Name}, unit: {ingredient.Unit}, Price/unit: {ingredient.PricePerUnit}, E/unit: {ingredient.EnergyPerUnit}, P/unit: {ingredient.ProteinPerUnit}");
//        }


//        dbConnection.Close();
//        return new Recipy(null, recipyName, ingredientsList, rec_desc);
//    }
//}



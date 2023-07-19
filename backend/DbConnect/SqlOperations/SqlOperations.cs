
using ClassLibrary;
using DbConnect.interfaces;
using DbConnect.Responses;
using Npgsql;
using System.Data.Common;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.SqlOperations;
public class SqlOperations
{
	private readonly NpgsqlDataSource dbDataSource;

	public IngredientsMethods IngredientsOps { get; }
	public CategoriesMethods CategoriesOps { get; }
	public RecipiesMethods RecipiesOps { get; }

	private readonly Sql.Sql sqlStrings;

	public SqlOperations() : this(false) { }
	public SqlOperations(bool devMode)
	{
		sqlStrings = new Sql.Sql(devMode);
		dbDataSource = NpgsqlDataSource.Create(ConnStr.Get(devMode));

		IngredientsOps = new(this, dbDataSource, sqlStrings);
		CategoriesOps = new(this, dbDataSource, sqlStrings);
		RecipiesOps = new(this, dbDataSource, sqlStrings);
	}

	public async Task<SqlResponse<List<IngredientInRecipyMap>>> GetIngredientsInRecipyMap(string type, int id)
	{
		if (type != "ingredient" && type != "recipy")
			return new SqlResponse<List<IngredientInRecipyMap>>(false, new(), "getIngredientsInRecipyMap(): Invalid type");

		try
		{
			using var command = dbDataSource.CreateCommand(type == "ingredient" ? sqlStrings.GetIngredientsInRecipiesMapByIngredient : sqlStrings.GetIngredientsInRecipiesMapByRecipy);
			command.Parameters.AddWithValue("@id", id);
			await using var reader = await command.ExecuteReaderAsync();

			List<IngredientInRecipyMap> mapList = new();
			while (await reader.ReadAsync())
			{
				mapList.Add(new(
					recipyId: reader.GetInt32(0),
					ingredientId: reader.GetInt32(1),
					quantity: reader.GetDouble(2)));
			}

			return new SqlResponse<List<IngredientInRecipyMap>>(true, mapList, $"Success!");
		}
		catch (Exception ex)
		{
			return new SqlResponse<List<IngredientInRecipyMap>>(false, new(), $"getIngredientsInRecipyMap(): Error: ${ex.Message}");
		}
	}
}

public class RecipiesMethods : IRecipyMethods
{
	private readonly SqlOperations parent;
	private readonly NpgsqlDataSource dbDataSource;
	private readonly Sql.Sql sqlStrings;

	public RecipiesMethods(SqlOperations parent, NpgsqlDataSource dbDataSource, Sql.Sql sqlStrings) { this.parent = parent; this.dbDataSource = dbDataSource; this.sqlStrings = sqlStrings; }

	public async Task<SqlResponse<List<Recipy>>> GetRecipyList()
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.GetRecipies);
			await using var reader = await command.ExecuteReaderAsync();

			List<Recipy> recipyList = new();
			while (await reader.ReadAsync())
			{
				var categoriesResponse = await parent.CategoriesOps.GetCategoriesForItem("recipy", reader.GetInt32(0));
				var ingredientsMapResponse = await parent.GetIngredientsInRecipyMap("recipy", reader.GetInt32(0));

				if (!categoriesResponse.Success || categoriesResponse.Data == null)
					return new SqlResponse<List<Recipy>>(false, new(), "GetRecipyList()" + categoriesResponse.Message);

				if (!ingredientsMapResponse.Success || ingredientsMapResponse.Data == null)
					return new SqlResponse<List<Recipy>>(false, new(), "GetRecipyList():" + ingredientsMapResponse.Message);

				recipyList.Add(new(
					id: reader.GetInt32(0),
					name: reader.GetString(1),
					ingredients: ingredientsMapResponse.Data.Select(mapping => new InredientReference(mapping.IngredientId, mapping.Quantity)).ToList(),
					inCategories: categoriesResponse.Data,
					description: reader.GetString(2)));
			}

			return new SqlResponse<List<Recipy>>(true, recipyList, $"Success!");
		}
		catch (Exception ex)
		{
			return new SqlResponse<List<Recipy>>(false, new List<Recipy>(), $"GetRecipyList(): Error: ${ex.Message}");
		}
	}

	public async Task<SqlResponse<Recipy?>> GetOneRecipyById(int recipyId)
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.GetRecipyById);
			command.Parameters.AddWithValue("@id", recipyId);
			await using var reader = await command.ExecuteReaderAsync();

			List<Recipy> recipyList = new();
			while (await reader.ReadAsync())
			{
				var categoriesResponse = await parent.CategoriesOps.GetCategoriesForItem("recipy", recipyId);
				var ingredientsMapResponse = await parent.GetIngredientsInRecipyMap("recipy", recipyId);

				if (!categoriesResponse.Success || categoriesResponse.Data == null)
					return new SqlResponse<Recipy?>(false, null, "GetOneRecipyById():" + categoriesResponse.Message);

				if (!ingredientsMapResponse.Success || ingredientsMapResponse.Data == null)
					return new SqlResponse<Recipy?>(false, null, "GetOneRecipyById():" + ingredientsMapResponse.Message);

				recipyList.Add(new(
					id: reader.GetInt32(0),
					name: reader.GetString(1),
					ingredients: ingredientsMapResponse.Data.Select(mapping => new InredientReference(mapping.IngredientId, mapping.Quantity)).ToList(),
					inCategories: categoriesResponse.Data,
					description: reader.GetString(2)));
			}

			return new SqlResponse<Recipy?>(true, recipyList.Count == 0 ? null : recipyList[0], $"Success!");
		}
		catch (Exception ex)
		{
			return new SqlResponse<Recipy?>(false, null, $"GetOneRecipyById(): Error: ${ex.Message}");
		}
	}

	public async Task<SqlResponse<Recipy>> DeleteRecipy(int recipyId)
	{
		throw new NotImplementedException();
	}

	public async Task<SqlResponse<Recipy>> InsertNewRecipy(Ingredient newRecipy)
	{
		throw new NotImplementedException();
	}

	public async Task<SqlResponse<Recipy>> UpdateRecipy(Recipy updatedRecipy, Recipy currentRecipy)
	{
		throw new NotImplementedException();
	}
}

public class IngredientsMethods : IIngredientsMethods
{
	private readonly SqlOperations parent;
	private readonly NpgsqlDataSource dbDataSource;
	private readonly Sql.Sql sqlStrings;

	public IngredientsMethods(SqlOperations parent, NpgsqlDataSource dbDataSource, Sql.Sql sqlStrings){ this.parent = parent; this.dbDataSource = dbDataSource; this.sqlStrings = sqlStrings; }

	public async Task<SqlResponse<List<Ingredient>>> GetIngredientsList()
	{
		try
		{
			using var command = dbDataSource.CreateCommand(sqlStrings.GetIngredients);
			await using var reader = await command.ExecuteReaderAsync();

			List<Ingredient> ingredientsList = new();
			while (await reader.ReadAsync())
			{
				var categoriesResponse = await parent.CategoriesOps.GetCategoriesForItem("ingredient", reader.GetInt32(0));

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
		catch (Exception ex)
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
				var categoriesResponse = await parent.CategoriesOps.GetCategoriesForItem("ingredient", ingredientId);

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
		var conn = await dbDataSource.OpenConnectionAsync();
		var categoryList = new List<Category>();

		foreach (var category in newIngredient.InCategories)
		{
			var response = await parent.CategoriesOps.InsertNewCategoryForItem("ingredient", category, conn);

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

			foreach (var cate in categoryList)
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
		if (updatedIngredient.Id == null || currentIngredient.Id == null)
			return new SqlResponse<Ingredient>(false, updatedIngredient, "No id provided");

		if (updatedIngredient.Id != currentIngredient.Id)
			return new SqlResponse<Ingredient>(false, updatedIngredient, "provedid ingredients data does not have matching id's");

		using var conn = await dbDataSource.OpenConnectionAsync();

		if (updatedIngredient.InCategories.Count != currentIngredient.InCategories.Count || !updatedIngredient.InCategories.All(currentIngredient.InCategories.Contains))
		{
			var response = await parent.CategoriesOps.GetAllCategoriesForItemType("ingredient");

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
					var response2 = await parent.CategoriesOps.InsertNewCategoryForItem("ingredient", cat, conn);

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

				foreach (var category in relevantCategories)
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

			var categoriesResponse = await parent.CategoriesOps.GetCategoriesForItem("ingredient", ingredientId);

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
		catch (Exception ex)
		{
			return new SqlResponse<Ingredient?>(false, null, ex.Message);
		}
	}
}

public class CategoriesMethods : ICategoriesMethods
{
	private readonly SqlOperations parent;
	private readonly NpgsqlDataSource dbDataSource;
	private readonly Sql.Sql sqlStrings;

	public CategoriesMethods(SqlOperations parent, NpgsqlDataSource dbDataSource, Sql.Sql sqlStrings) { this.parent = parent; this.dbDataSource = dbDataSource; this.sqlStrings = sqlStrings; }

	public async Task<SqlResponse<List<Category>>> GetAllCategoriesForItemType(string categoryType)
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

	public async Task<SqlResponse<Category>> InsertNewCategoryForItem(string categoryType, string newCategoryName, NpgsqlConnection conn)
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
		catch (Exception ex)
		{
			return new SqlResponse<Category>(false, new(), message: ex.Message);
		}
	}

	public async Task<SqlResponse<List<string>>> GetCategoriesForItem(string categoryType, int itemId)
	{
		if (categoryType != "recipy" && categoryType != "ingredient")
			return new SqlResponse<List<string>>(false, new List<string>(), "GetCategoriesForItem(): Trying to retrieve non-existant category types");

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
		catch (Exception ex)
		{
			return new SqlResponse<List<string>>(false, new List<string>(), "GetCategoriesForItem(): " + ex.Message);
		}
		finally
		{
			conn.Close();
		}
	}
}
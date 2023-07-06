using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Sql;
public class Sql
{
	public Sql(bool dbLocal)
	{
		var recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
		var ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
		var ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";

		var recipyCategoriesTableName = dbLocal ? "stored_recipy_categories" : "thrifty_helper__stored_recipy_categories";
		var categoriesInRecipiesTableName = dbLocal ? "categories_in_recipies" : "thrifty_helper__categories_in_recipies";

		var ingredientCategoriesTableName = dbLocal ? "stored_ingredient_categories" : "thrifty_helper__stored_ingredient_categories";
		var categoriesInIngredientsTableName = dbLocal ? "categories_in_ingredients" : "thrifty_helper__categories_in_ingredients";

		TableNames = new string[] { recipyTableName, ingredientsTableName, ingredientsInRecipiesTableName, recipyCategoriesTableName, categoriesInRecipiesTableName, ingredientCategoriesTableName, categoriesInIngredientsTableName};

		GetIngredients = $@"SELECT * FROM {ingredientsTableName};";

		GetIngredientById = $@"
			SELECT * FROM {ingredientsTableName}
			WHERE ingredient_id = @i_id
		;"; 

		GetCategoriesFromIngredientId = $@"
			SELECT c_t.category_name 
			FROM {categoriesInIngredientsTableName} c_i_t
				INNER JOIN {ingredientCategoriesTableName} c_t
					ON c_i_t.ingredient_category_id = c_t.ingredient_category_id
			WHERE c_i_t.ingredient_id = @id 
		;";

		GetCategoriesFromRecipyId = $@"
			SELECT category_name 
			FROM {categoriesInRecipiesTableName}
			WHERE recipy_id = @id
		;";

		GetRecipies = $@"
			SELECT st.*, ip.*, si.*
			FROM {recipyTableName} st
				INNER JOIN {ingredientsInRecipiesTableName} ip
					ON st.recipy_id = ip.recipy_id
				INNER JOIN {ingredientsTableName} si
					ON ip.ingredient_id = si.ingredient_id;
		";

		InsertNewIngredientCategory = $@"
			INSERT INTO {ingredientCategoriesTableName}(
				category_name
			)
			VALUES(
				@c_n
			)
			ON CONFLICT DO NOTHING
			RETURNING *
		;";

		InsertNewRecipyCategory = $@"
			INSERT INTO {recipyCategoriesTableName}(
				category_name
			)
			VALUES(
				@c_n
			)
			ON CONFLICT DO NOTHING
			RETURNING *
		;";

		InsertNewIngredient = $@"
			INSERT INTO {ingredientsTableName}
			(
				ingredient_name,
				ingredient_unit,
				price_per_unit,
				energy_per_unit,
				protein_per_unit,
				last_updated
			)
			VALUES(
				@i_n,
				@i_u,
				@prPU,
				@ePU,
				@pPU,
				CURRENT_TIMESTAMP
			)
			ON CONFLICT DO NOTHING
			RETURNING *
		;";

		InsertIngredientCategoryMapping = @$"
			INSERT INTO {categoriesInIngredientsTableName}
			(
				ingredient_category_id,
				ingredient_id
			)
			VALUES(
				@i_c_id,
				@i_id
			)
			ON CONFLICT DO NOTHING;
		";

		UpdateIngredient = @$"
			UPDATE {ingredientsTableName}
			SET 
				ingredient_name = @i_n,
				ingredient_unit = @i_u,
				price_per_unit = @prPU,
				energy_per_unit = @ePU,
				protein_per_unit = @pPU,
				last_updated = CURRENT_TIMESTAMP
			WHERE 
				ingredient_id = @i_id
			RETURNING *;
		";

		GetAllIngredientCategories = $@"SELECT * FROM {ingredientCategoriesTableName};";
		GetAllRecipyCategories = $@"SELECT * FROM {recipyCategoriesTableName};";

		DeleteOldIngredintCategoryMappings = $@"
			DELETE FROM {categoriesInIngredientsTableName}
			WHERE ingredient_id = @i_id;
		";

		InsertNewIngredientCategoryMapping = $@"
			INSERT INTO {categoriesInIngredientsTableName}(
				ingredient_id,
				ingredient_category_id
			)
			VALUES(
				@i_id,
				@i_c_id
			)
			ON CONFLICT DO NOTHING;
		";

		DeleteIngredient = $@"DELETE FROM {ingredientsTableName} WHERE ingredient_id = @i_id;";
	}

	private string[] TableNames { get; }
	public string GetIngredients { get; }
	public string GetIngredientById { get; }
	public string GetCategoriesFromIngredientId { get; }
	public string GetCategoriesFromRecipyId { get; }
	public string GetRecipies { get; }
	public string InsertNewIngredientCategory { get; }
	public string InsertNewRecipyCategory { get; }
	public string InsertNewIngredient { get; }
	public string InsertIngredientCategoryMapping { get; }

	public string UpdateIngredient { get;}
	public string GetAllIngredientCategories { get; }
	public string GetAllRecipyCategories { get; }
	public string DeleteOldIngredintCategoryMappings { get; }
	public string InsertNewIngredientCategoryMapping { get; }
	public string DeleteIngredient { get; }
}

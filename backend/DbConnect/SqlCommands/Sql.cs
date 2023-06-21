using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbConnect.Sql;
internal class Sql
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

		//GetIngredients = $@"
		//	SELECT * 
		//	FROM {ingredientsTableName};
		//";

		GetIngredients = $@"SELECT * FROM {ingredientsTableName};";

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
	}

	public string GetIngredients { get; }
	public string GetCategoriesFromIngredientId { get; }
	public string GetCategoriesFromRecipyId { get; }
	public string GetRecipies { get; }
}

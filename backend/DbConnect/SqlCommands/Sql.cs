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

		//GetIngredients = $@"
		//	SELECT * 
		//	FROM {ingredientsTableName};
		//";

		GetIngredients = $@"
			SELECT ing_t.*
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
	public string GetRecipies { get; }
}

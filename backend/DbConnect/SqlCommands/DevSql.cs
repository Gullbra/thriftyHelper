namespace DbConnect.Sql;

internal class DevSql
{
	public DevSql(bool dbLocal)
	{
		var recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
		var ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
		var ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";

		var recipyCategories = dbLocal ? "stored_recipy_categories" : "thrifty_helper__stored_recipy_categories";
		var categoriesInRecipies = dbLocal ? "categories_in_recipies" : "thrifty_helper__categories_in_recipies";

		var ingredientCategories = dbLocal ? "stored_ingredient_categories" : "thrifty_helper__stored_ingredient_categories";
		var categoriesInIngredients = dbLocal ? "categories_in_ingredients" : "thrifty_helper__categories_in_ingredients";

		TablesNamesList = new() { recipyTableName, ingredientsTableName, ingredientsInRecipiesTableName, recipyCategories, categoriesInRecipies, ingredientCategories, categoriesInIngredients };

		DevTestConnection = @$"
			SELECT tablename 
			FROM pg_catalog.pg_tables 
			WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema';";

		SetUpTables = $@"
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
				recipy_id 				integer 			REFERENCES {recipyTableName} ON DELETE RESTRICT,
				ingredient_id			integer 			REFERENCES {ingredientsTableName} ON DELETE RESTRICT,
				PRIMARY KEY(recipy_id, ingredient_id),
				quantity 					float(4)			NOT NULL	
			);

			CREATE TABLE IF NOT EXISTS {recipyCategories} (
				recipy_category_id	SERIAL			PRIMARY KEY,
				category_name				VARCHAR(32)	NOT NULL,
				UNIQUE(recipy_category_id, category_name)
			);

			CREATE TABLE IF NOT EXISTS {categoriesInRecipies} (
				recipy_id 					integer 			REFERENCES {recipyTableName} ON DELETE RESTRICT,
				recipy_category_id	integer 			REFERENCES {recipyCategories} ON DELETE RESTRICT,
				PRIMARY KEY(recipy_id, recipy_category_id)
			);

			CREATE TABLE IF NOT EXISTS {ingredientCategories} (
				ingredient_category_id	SERIAL			PRIMARY KEY,
				category_name						VARCHAR(32)	NOT NULL,
				UNIQUE(ingredient_category_id, category_name)
			);

			CREATE TABLE IF NOT EXISTS {categoriesInIngredients} (
				ingredient_id 					integer 			REFERENCES {ingredientsTableName} ON DELETE RESTRICT,
				ingredient_category_id	integer 			REFERENCES {ingredientCategories} ON DELETE RESTRICT,
				PRIMARY KEY(ingredient_id, ingredient_category_id)
			);";

		DevDropTables = @$"
			DROP TABLE IF EXISTS 
				{ingredientsInRecipiesTableName}, 
				{recipyTableName}, 
				{ingredientsTableName}, 

				{categoriesInIngredients},
				{ingredientCategories},

				{categoriesInRecipies},
				{recipyCategories};";
	}

	public List<string> TablesNamesList { get; }
	public string DevTestConnection { get; }
	public string SetUpTables { get; }
	public string DevDropTables { get; }
}
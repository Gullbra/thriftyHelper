namespace DbConnect.Sql;

internal class DevSql
{
	public DevSql(bool dbLocal)
	{
		var recipyTableName = dbLocal ? "stored_recipies" : "thrifty_helper__stored_recipies";
		var ingredientsTableName = dbLocal ? "stored_ingredients" : "thrifty_helper__stored_ingredients";
		var ingredientsInRecipiesTableName = dbLocal ? "ingredients_in_recipies" : "thrifty_helper__ingredients_in_recepies";

		var recipyCategoriesTableName = dbLocal ? "stored_recipy_categories" : "thrifty_helper__stored_recipy_categories";
		var categoriesInRecipiesTableName = dbLocal ? "categories_in_recipies" : "thrifty_helper__categories_in_recipies";

		var ingredientCategoriesTableName = dbLocal ? "stored_ingredient_categories" : "thrifty_helper__stored_ingredient_categories";
		var categoriesInIngredientsTableName = dbLocal ? "categories_in_ingredients" : "thrifty_helper__categories_in_ingredients";

		TablesNamesList = new() 
		{ 
			new KeyValuePair<string, string>("recipyTable", recipyTableName),
			new KeyValuePair<string, string>("ingredientsTable", ingredientsTableName),
			new KeyValuePair<string, string>("ingredients in recipies", ingredientsInRecipiesTableName),

			new KeyValuePair<string, string>("recipyCategoriesTable", recipyCategoriesTableName),
			new KeyValuePair<string, string>("ingredientCategoryTable", ingredientCategoriesTableName),

			new KeyValuePair<string, string>("categories in recipies", categoriesInRecipiesTableName),
			new KeyValuePair<string, string>("ingredients in categories", categoriesInIngredientsTableName)
		};

		DevTestConnection = @$"
			SELECT tablename 
			FROM pg_catalog.pg_tables 
			WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema';";

		SetUpTables = $@"
			CREATE TABLE IF NOT EXISTS {recipyTableName} (
				recipy_id       SERIAL        PRIMARY KEY,
				recipy_name     varchar(120)  NOT NULL,
				description     text          NOT NULL,
				UNIQUE (recipy_id),
				UNIQUE (recipy_name)
			);

			CREATE TABLE IF NOT EXISTS {ingredientsTableName} (
				ingredient_id   	SERIAL        PRIMARY KEY,
				ingredient_name   varchar(255)  NOT NULL,
				ingredient_unit		varchar(20)   NOT NULL,
				price_per_unit		float(8)			NOT NULL,
				energy_per_unit		float(8)			NOT NULL,
				protein_per_unit	float(8)			NOT NULL,
				UNIQUE (ingredient_id),
				UNIQUE (ingredient_name)
			);

			CREATE TABLE IF NOT EXISTS {ingredientsInRecipiesTableName} (
				recipy_id 				integer 			REFERENCES {recipyTableName} ON DELETE RESTRICT,
				ingredient_id			integer 			REFERENCES {ingredientsTableName} ON DELETE RESTRICT,
				PRIMARY KEY(recipy_id, ingredient_id),
				quantity 					float(4)			NOT NULL	
			);

			CREATE TABLE IF NOT EXISTS {recipyCategoriesTableName} (
				recipy_category_id	SERIAL			PRIMARY KEY,
				category_name				VARCHAR(32)	NOT NULL,
				UNIQUE (recipy_category_id),
				UNIQUE (category_name)
			);

			CREATE TABLE IF NOT EXISTS {categoriesInRecipiesTableName} (
				recipy_id 					integer 			REFERENCES {recipyTableName} ON DELETE RESTRICT,
				recipy_category_id	integer 			REFERENCES {recipyCategoriesTableName} ON DELETE RESTRICT,
				PRIMARY KEY(recipy_id, recipy_category_id)
			);

			CREATE TABLE IF NOT EXISTS {ingredientCategoriesTableName} (
				ingredient_category_id	SERIAL			PRIMARY KEY,
				category_name						VARCHAR(32)	NOT NULL,
				UNIQUE (ingredient_category_id),
				UNIQUE (category_name)
			);

			CREATE TABLE IF NOT EXISTS {categoriesInIngredientsTableName} (
				ingredient_id 					integer 			REFERENCES {ingredientsTableName} ON DELETE RESTRICT,
				ingredient_category_id	integer 			REFERENCES {ingredientCategoriesTableName} ON DELETE RESTRICT,
				PRIMARY KEY(ingredient_id, ingredient_category_id)
			);";

		DevDropTables = @$"
			DROP TABLE IF EXISTS 
				{ingredientsInRecipiesTableName}, 
				{recipyTableName}, 
				{ingredientsTableName}, 

				{categoriesInIngredientsTableName},
				{ingredientCategoriesTableName},

				{categoriesInRecipiesTableName},
				{recipyCategoriesTableName};";

		DevInsertCategoryIng = $@"
			INSERT INTO {ingredientCategoriesTableName}()
			VALUES()		
		"; 

	}

	public List<KeyValuePair<string, string>> TablesNamesList { get; }
	public string DevTestConnection { get; }
	public string SetUpTables { get; }
	public string DevDropTables { get; }
	public string DevInsertCategoryIng { get; }
}
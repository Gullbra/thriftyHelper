using Npgsql;
using System;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.DbConnect;

/*
 https://zetcode.com/csharp/postgresql/
 */

/* Tables:
stored_recipies(
    recipy_id       SERIAL        PRIMARY KEY,
    name            varchar(120)  NOT NULL,
    description     text          NOT NULL
);

stored_ingredients
(
  ingredient_id   	SERIAL          PRIMARY KEY,
	name            	varchar(255)  NOT NULL,
  unit		    			varchar(20)   NOT NULL,
	price_per_unit		float(8)			NOT NULL,
	energy_per_unit		float(8)			NOT NULL,
	protein_per_unit	float(8)			NOT NULL
);

ingredients_in_recepies
(
	recipy_id 				integer 			REFERENCES stored_recipies,
	ingredient_id			integer 			REFERENCES stored_ingredients,
  PRIMARY KEY(recipy_id, ingredient_id),
	quantity 					float(4)			NOT NULL	
);
 */

/* Insert Ingredient
 INSERT INTO stored_ingredients (
	 name, 
	 unit, 
	 price_per_unit, 
	 energy_per_unit,
	 protein_per_unit
 ) VALUES (
	 'ägg',
	 'st(ca 60g)',
	 2,
	 82.2,
	 7.44
 );
*/

/* Insert recipy
	INSERT INTO stored_recipies (
		name, 
		description
	) VALUES (
		'blodpudding med ägg och bacon',
		'Blodpudding med äggröra och stekt bacon'
	);
 */

/* Insert ingredients in recipy referece
 * 	foreach ingredient in recipy:

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

// For testing through commandline:
//SqlOperations.GetRecipy("blodpudding med ägg och bacon");
Console.WriteLine("\tTesting new:");
var TestObj = new NewSqlOperations(true);
TestObj.TestingConnection();
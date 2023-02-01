using DbConnect;
using Npgsql;
using System;
using System.Data.SqlTypes;
using ThriftyHelper.Backend.DbConnect;

/*
 https://zetcode.com/csharp/postgresql/
 */

Sectrets ENV = new();

string connectionString = $"Host={ENV.Host};Username={ENV.Username};Password={ENV.Password};Database={ENV.Database}";
using NpgsqlConnection dbConnection = new(connectionString);

dbConnection.Open();

/* Tables:
stored_recipies(
    recipy_id       SERIAL        PRIMARY KEY,
    name            varchar(120)  NOT NULL,
    description     text          NOT NULL
);
stored_ingredients
(
  ingredient_id   	SERIAL        PRIMARY KEY,
	name            	varchar(255)  NOT NULL,
  unit		    	    varchar(20)   NOT NULL,
	price_per_unit		float(4)		  NOT NULL,
	energy_per_unit		integer			  NOT NULL,
	protein_per_unit	float(4)		  NOT NULL
);
ingredients_in_recepies
(
	recipy_id 		integer 	REFERENCES stored_recipies,
	ingredient_id integer 	REFERENCES stored_ingredients,
  PRIMARY KEY(recipy_id, ingredient_id),
	quantity 			float(4)	NOT NULL	
);
 */

// SqlOperations.GetSqlVersion(dbConnection);

// SqlOperations.CreateTable(dbConnection);

// SqlOperations.InsertRow(dbConnection);

// SqlOperations.GetAllColumns(dbConnection);

// SqlOperations.GetAllWithColumnHeaders(dbConnection);

dbConnection.Close();
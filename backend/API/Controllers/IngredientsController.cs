using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThriftyHelper.Backend.ClassLibrary;
using ThriftyHelper.Backend.DbConnect;

namespace ThriftyHelper.Backend.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IngredientsController : ControllerBase
{
  private readonly SqlOperations Db;

	public IngredientsController (SqlOperations db) { Db = db; }

	[EnableCors("ReactDevEnv")]
	[HttpGet()]
	public List<Ingredient>? GetIngredients()
	{
		return Db.GetIngredientsList();
	}

	[EnableCors("ReactDevEnv")]
	[HttpGet("{name}")]
	public Ingredient? GetIngredientByName(string name)
	{
		return Db.GetIngredientByName(name);
	}

}
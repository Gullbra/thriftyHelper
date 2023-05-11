using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThriftyHelper.Backend.ClassLibrary;
using ThriftyHelper.Backend.DbConnect;

namespace ThriftyHelper.Backend.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RecipiesController : ControllerBase
{
	/*
		Convert to IActionResult?
	 */

  private readonly SqlOperations Db;

	public RecipiesController (SqlOperations db) { Db = db; }


	[EnableCors("ReactDevEnv")]
	[HttpGet()]
	public List<Recipy>? GetRecipyList()
	{
		return Db.GetRecipyList();
	}

	[EnableCors("ReactDevEnv")]
	[HttpGet("{name}")]
	public Recipy? GetRecipyByName(string name)
	{
		return Db.GetRecipyByName(name);
	}

}


//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//	public class IngredientsController : Controller
//	{
//		public IActionResult Index()
//		{
//			return View();
//		}
//	}
//}

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
	// private readonly SqlOperations _logger;

	[EnableCors("ReactDevEnv")]
	[HttpGet()]
	public List<Ingredient>? GetIngredients()
	{
		return new SqlOperations(true).GetIngredientsList();
	}

	[EnableCors("ReactDevEnv")]
	[HttpGet("{name}")]
	public Ingredient? GetIngredientByName(string name)
	{
		return new SqlOperations(true).GetIngredientByName(name);
	}

}
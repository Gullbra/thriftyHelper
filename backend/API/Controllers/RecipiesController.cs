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
	[EnableCors("ReactDevEnv")]
	[HttpGet("{name}")]
	public Recipy GetRecipyByName(string name)
	{
		return SqlOperations.GetRecipy(name);
	}
}


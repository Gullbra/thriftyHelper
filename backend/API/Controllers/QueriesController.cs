using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController : ControllerBase
{
	[HttpGet]
	public List<Car> GetTable()
	{
		return
	}
}


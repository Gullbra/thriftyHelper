using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThriftyHelper.Backend.ClassLibrary;

namespace DbConnect.Responses;

public class SqlResponse<TReturn>
{
	public SqlResponse(bool success, TReturn data, string message)
	{ Success = success; Data = data; Message = message; }

	public bool Success { get; }
	public TReturn Data { get; }
	public string Message { get; }

	//public SqlResponse(bool success, Ingredient data, string message)
	//{ Success = success; DataIngredient = data; Message = message; }
	//public SqlResponse(bool success, List<Ingredient> data, string message)
	//{ Success = success; DataIngredientList = data; Message = message; }
	//public SqlResponse(bool success, Recipy data, string message)
	//{ Success = success; DataRecipy = data; Message = message; }
	//public SqlResponse(bool success, List<Recipy> data, string message)
	//{ Success = success; DataRecipyList = data; Message = message; }
	//public SqlResponse(bool success, List<Category> data, string message)
	//{ Success = success; DataCategoryList = data; Message = message; }
	//public SqlResponse(bool success, List<string> data, string message)
	//{ Success = success; DataStringList = data; Message = message; }

	//public bool Success { get; }
	//public List<string>? DataStringList { get; }
	//public List<Category>? DataCategoryList { get; }
	//public Ingredient? DataIngredient { get; }
	//public List<Ingredient>? DataIngredientList { get; }
	//public Recipy? DataRecipy { get; }
	//public List<Recipy>? DataRecipyList { get; }
	//public string Message { get; }
}

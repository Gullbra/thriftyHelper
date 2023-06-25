using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftyHelper.Backend.ClassLibrary;

public class Category
{
	public Category() { }
	public Category(string categoryName, int categoryId)
	{ CategoryName = categoryName; CategoryId = categoryId; }

	public string? CategoryName { get; }
	public int? CategoryId { get; }
}

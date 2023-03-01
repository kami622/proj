using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult NotFound(int code)
		{
			return View();
		}
	}
}

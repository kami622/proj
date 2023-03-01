using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher,student")]
	public class MainController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

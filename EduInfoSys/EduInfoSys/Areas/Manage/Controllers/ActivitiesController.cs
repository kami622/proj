using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin, admin")]
	public class ActivitiesController : Controller
	{
		private readonly AppDbContext _context;
		public ActivitiesController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Activities";
			List<Log> logs = await _context.Logs.OrderByDescending(l => l.Date).ToListAsync();
			return View(logs);
		}
	}
}

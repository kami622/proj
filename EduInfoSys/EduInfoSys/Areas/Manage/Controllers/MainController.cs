using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class MainController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public MainController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Main";
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			UserVM userVM = new()
			{
				User = user,
				Role = (await _userManager.IsInRoleAsync(user, "superadmin") ? "Super Administrator" : "Administrator")
			};
			ViewBag.Logs = await _context.Logs.OrderBy(l => l.Date).ToListAsync();
			ViewBag.Groups = await _context.Groups.CountAsync();
			ViewBag.Departments = await _context.Departments.CountAsync();
			IList<AppUser> users = await _userManager.GetUsersInRoleAsync("student");
			ViewBag.Students = users.Count();
			users = await _userManager.GetUsersInRoleAsync("teacher");
			ViewBag.Teachers = users.Count();
			return View(userVM);
		}
	}
}

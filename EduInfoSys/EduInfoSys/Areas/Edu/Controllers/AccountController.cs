using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher,student")]
	public class AccountController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public AccountController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Profile()
		{
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			string role = string.Empty;
			if (await _userManager.IsInRoleAsync(user, "teacher")) role = "Teacher";
			else role = "Student";
			return View(new UserVM { User = user, Role = role });
		}

		public async Task<IActionResult> Notifications()
		{
			return View();
		}
	}
}

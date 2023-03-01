using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher,student")]
	public class SettingsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public SettingsController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			SettingsVM setVM = new()
			{
				Customization = await _context.Customizations.Include(c => c.User).FirstOrDefaultAsync(c => c.User == user)
			};
			return View(setVM);
		}

		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody]ChangePassVM passVM)
		{
			if (!ModelState.IsValid) return Json(ModelState.Values.First().Errors.First().ErrorMessage);
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user is null) return NotFound();
			var result = await _userManager.ChangePasswordAsync(user, passVM.CurrentPassword, passVM.NewPassword);
			if (!result.Succeeded) return Json(result.Errors.First().Description);
			return Json("ok");
		}

		[HttpPost]
		public async Task<IActionResult> Customize([FromBody]CustomizationVM customVM)
		{
			if (!ModelState.IsValid) return Json(ModelState.Values.First().Errors.First().ErrorMessage);
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (user is null) return NotFound();
			if (customVM.ColorScheme != "default" && customVM.ColorScheme != "dark" && customVM.ColorScheme != "light" && customVM.ColorScheme != "colored") return Json("Invalid theme");
			if (customVM.SidebarLayout != "normal" && customVM.SidebarLayout != "compact") return Json("Invalid sidebar layout");
			if (customVM.SidebarPosition != "left" && customVM.SidebarPosition != "right") return Json("Invalid sidebar position");
			if (customVM.Layout != "fluid" && customVM.Layout != "boxed") return Json("Invalid layout");
			Customization found = await _context.Customizations.FirstOrDefaultAsync(c => c.User.Id == user.Id);
			if (found is null)
			{
				await _context.Customizations.AddAsync(new() { ColorScheme = customVM.ColorScheme, SidebarLayout = customVM.SidebarLayout, SidebarPosition = customVM.SidebarPosition, Layout = customVM.Layout, User = user });
			}
			else
			{
				found.ColorScheme = customVM.ColorScheme;
				found.SidebarLayout = customVM.SidebarLayout;
				found.SidebarPosition = customVM.SidebarPosition;
				found.Layout = customVM.Layout;
			}
			await _context.SaveChangesAsync();
			return Json("ok");
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduInfoSys.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly AppDbContext _context;
		public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, AppDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_signInManager = signInManager;
			_context = context;
		}

		//public async Task<IActionResult> CreationSeed()
		//{
		//	await _roleManager.CreateAsync(new IdentityRole { Name = "superadmin" });
		//	await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });
		//	await _roleManager.CreateAsync(new IdentityRole { Name = "teacher" });
		//	await _roleManager.CreateAsync(new IdentityRole { Name = "student" });
		//	AppUser user = new()
		//	{
		//		UserName = "superadmin",
		//		FirstName = "Super",
		//		LastName = "Admin",
		//		PhotoPath = "no-photo.jpg",
		//		CreatedAt = DateTime.Now,
		//		UpdatedAt = DateTime.Now
		//	};
		//	await _userManager.CreateAsync(user, "Qwerty-123");
		//	await _userManager.AddToRoleAsync(user, "superadmin");
		//	return Ok();
		//}

		public IActionResult Index()
		{
			return RedirectToAction("login");
		}

		public async Task<IActionResult> Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
				if (user is not null)
				{
					string area = string.Empty;
					if (await _userManager.IsInRoleAsync(user, "superadmin") || await _userManager.IsInRoleAsync(user, "admin")) area = "manage";
					else area = "edu";
					return RedirectToAction("index", "main", new { area = area });
				}
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginVM loginVM)
		{
			if (User.Identity.IsAuthenticated)
			{
				AppUser found = await _userManager.FindByNameAsync(User.Identity.Name);
				if (found is not null) return RedirectToAction("login");
			}
			if (!ModelState.IsValid) return View(loginVM);
			AppUser user = await _userManager.FindByNameAsync(loginVM.UserName);
			if (user is null)
			{
				ModelState.AddModelError("", "Username or password is incorrect");
				return View(loginVM);
			}
			var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.RememberMe, false);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Username or password is incorrect");
				return View(loginVM);
			}
			user.LastSignIn = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Signed in", Text = $"{user.UserName} signed in.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("login");
		}

		public async Task<IActionResult> Logout()
		{
			if (!User.Identity.IsAuthenticated) return RedirectToAction("login");
			await _context.Logs.AddAsync(new Log { Heading = "Signed out", Text = $"{User.Identity.Name} signed out.", Date = DateTime.Now });
			await _signInManager.SignOutAsync();
			await _context.SaveChangesAsync();
			return RedirectToAction("login");
		}
	}
}

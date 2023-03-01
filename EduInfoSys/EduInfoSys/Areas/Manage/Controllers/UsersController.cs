using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class UsersController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IWebHostEnvironment _env;
		public UsersController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
		{
			_context = context;
			_userManager = userManager;
			_env = env;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Users";
			ViewBag.IsSuper = false;
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin")) ViewBag.IsSuper = true;
			List<AppUser> users = await _context.Users.ToListAsync();
			List<UserVM> userVMs = new();
			foreach (AppUser user in users)
			{
				string role = string.Empty;
				if (await _userManager.IsInRoleAsync(user, "superadmin")) role = "Super Administrator";
				else if (await _userManager.IsInRoleAsync(user, "admin")) role = "Administrator";
				else if (await _userManager.IsInRoleAsync(user, "teacher")) role = "Teacher";
				else if (await _userManager.IsInRoleAsync(user, "student")) role = "Student";
				userVMs.Add(new() { User = user, Role = role });
			}
			return View(userVMs);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Page = "Users";
			ViewBag.IsSuper = false;
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin")) ViewBag.IsSuper = true;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(UserCreateVM userVM)
		{
			ViewBag.IsSuper = false;
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin")) ViewBag.IsSuper = true;
			if (!ModelState.IsValid) return View(userVM);
			if (userVM.Role != "student" && userVM.Role != "teacher")
			{
				if (userVM.Role != "admin" || !ViewBag.IsSuper)
				{
					ModelState.AddModelError("Role", "Incorrect choice");
					return View(userVM);
				}
			}
			AppUser user = null;
			user = await _userManager.FindByNameAsync(userVM.UserName);
			if (user is not null)
			{
				ModelState.AddModelError("UserName", "Username has taken.");
				return View(userVM);
			}
			if (userVM.Email is not null)
			{
				user = await _userManager.FindByEmailAsync(userVM.Email);
				if (user is not null)
				{
					ModelState.AddModelError("Email", "User with this email has already been created");
					return View(userVM);
				}
			}
			string photoPath = "no-photo.jpg";
			if (userVM.ImageFile is not null)
			{
				(bool check, string msg) result = ImageHelper.Check(userVM.ImageFile);
				if (!result.check)
				{
					ModelState.AddModelError("ImageFile", result.msg);
					return View(userVM);
				}
				photoPath = ImageHelper.Save(_env.WebRootPath, "assets/img/avatars", userVM.ImageFile);
			}
			user = new()
			{
				FirstName = userVM.FirstName,
				LastName = userVM.LastName,
				UserName = userVM.UserName,
				IsFemale = userVM.Gender == "female",
				PhotoPath = photoPath,
				Email = userVM.Email,
				PhoneNumber = userVM.PhoneNumber,
				BirthDate = userVM.BirthDate,
				AdditionalInfo = userVM.AdditionalInfo,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			var attempt = await _userManager.CreateAsync(user, userVM.Password);
			if (!attempt.Succeeded)
			{
				ModelState.AddModelError("", attempt.Errors.FirstOrDefault().Description);
				if (photoPath != "no-photo.jpg") ImageHelper.Delete(_env.WebRootPath, "assets/img/avatars", photoPath);
				return View(userVM);
			}
			await _userManager.AddToRoleAsync(user, userVM.Role);
			await _context.Logs.AddAsync(new Log { Heading = "Created user", Text = $"{User.Identity.Name} created new user [ID: {user.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(string id)
		{
			ViewBag.Page = "Users";
			ViewBag.IsSuper = false;
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin")) ViewBag.IsSuper = true;
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user is null) return NotFound();
			if (await _userManager.IsInRoleAsync(user, "superadmin")) return NotFound();
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "admin") && await _userManager.IsInRoleAsync(user, "admin")) return NotFound();
			string role = string.Empty;
			if (await _userManager.IsInRoleAsync(user, "admin")) role = "admin";
			else if (await _userManager.IsInRoleAsync(user, "teacher")) role = "teacher";
			else role = "student";
			UserUpdateVM userVM = new()
			{
				Id = user.Id,
				UserName = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				Role = role,
				Gender = user.IsFemale ? "female" : "male",
				AdditionalInfo = user.AdditionalInfo,
				PhoneNumber = user.PhoneNumber,
				PhotoPath = user.PhotoPath,
				BirthDate = user.BirthDate
			};
			return View(userVM);
		}
		[HttpPost]
		public async Task<IActionResult> Update(UserUpdateVM userVM)
		{
			ViewBag.IsSuper = false;
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin")) ViewBag.IsSuper = true;
			AppUser user = await _userManager.FindByIdAsync(userVM.Id);
			if (user is null) return NotFound();
			userVM.PhotoPath = user.PhotoPath;
			if (!ModelState.IsValid) return View(userVM);
			if (userVM.Role != "student" && userVM.Role != "teacher")
			{
				if (userVM.Role != "admin" || !ViewBag.IsSuper)
				{
					ModelState.AddModelError("Role", "Incorrect choice");
					return View(userVM);
				}
			}
			if (user.UserName != userVM.UserName)
			{
				AppUser found = await _userManager.FindByNameAsync(userVM.UserName);
				if (found is not null)
				{
					ModelState.AddModelError("UserName", "Username has taken");
					return View(userVM);
				}
			}
			if (user.Email != userVM.Email)
			{
				if (userVM.Email is not null)
				{
					AppUser found = await _userManager.FindByEmailAsync(userVM.Email);
					if (found is not null)
					{
						ModelState.AddModelError("Email", "User with this email has already been created");
						return View(userVM);
					}
				}
			}
			if (userVM.ImageFile is not null)
			{
				(bool check, string msg) result = ImageHelper.Check(userVM.ImageFile);
				if (!result.check)
				{
					ModelState.AddModelError("ImageFile", result.msg);
					return View(userVM);
				}
				if (user.PhotoPath != "no-photo.jpg") ImageHelper.Delete(_env.WebRootPath, "assets/img/avatars", user.PhotoPath);
				user.PhotoPath = ImageHelper.Save(_env.WebRootPath, "assets/img/avatars", userVM.ImageFile);
			}
			if (!await _userManager.IsInRoleAsync(user, userVM.Role))
			{
				await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
				await _userManager.AddToRoleAsync(user, userVM.Role);
			}
			user.FirstName = userVM.FirstName;
			user.LastName = userVM.LastName;
			user.Email = userVM.Email;
			user.PhoneNumber = userVM.PhoneNumber;
			user.UserName = userVM.UserName;
			user.IsFemale = userVM.Gender == "female";
			user.AdditionalInfo = userVM.AdditionalInfo;
			user.BirthDate = userVM.BirthDate;
			user.UpdatedAt = DateTime.Now;
			await _userManager.UpdateAsync(user);
			await _context.Logs.AddAsync(new Log { Heading = "Updated user", Text = $"{User.Identity.Name} updated user [ID: {user.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(string id)
		{
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user is null) return NotFound();
			if (await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "admin") && await _userManager.IsInRoleAsync(user, "admin")) return NotFound();
			if (await _userManager.IsInRoleAsync(user, "superadmin")) return NotFound();
			if (user.PhotoPath != "no-photo.jpg") ImageHelper.Delete(_env.WebRootPath, "assets/img/avatars", user.PhotoPath);
			_context.Users.Remove(user);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted user", Text = $"{User.Identity.Name} deleted user [ID: {user.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return Ok();
		}

		public async Task<IActionResult> Details(string id)
		{
			AppUser user = await _userManager.FindByIdAsync(id);
			if (user is null) return NotFound();
			string role = string.Empty;
			if (await _userManager.IsInRoleAsync(user, "superadmin")) role = "Super Administrator";
			else if (await _userManager.IsInRoleAsync(user, "admin")) role = "Administrator";
			else if (await _userManager.IsInRoleAsync(user, "teacher")) role = "Teacher";
			else if (await _userManager.IsInRoleAsync(user, "student")) role = "Student";
			UserVM userVM = new()
			{
				User = user,
				Role = role
			};
			ViewBag.IsSuper = await _userManager.IsInRoleAsync(await _userManager.FindByNameAsync(User.Identity.Name), "superadmin");
			return PartialView("_UserDetailsPartial", userVM);
		}
	}
}

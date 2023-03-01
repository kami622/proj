using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class StudentsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public StudentsController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Students";
			List<GroupStudent> students = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).Include(gs => gs.Group.Department).ToListAsync();
			return View(students);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Page = "Students";
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("student");
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(StudentCreateVM studentVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("student");
			if (!ModelState.IsValid) return View(studentVM);
			AppUser user = await _userManager.FindByIdAsync(studentVM.UserId);
			if (user is null)
			{
				ModelState.AddModelError("UserId", "Invalid choice");
				return View(studentVM);
			}
			Group group = await _context.Groups.FindAsync(studentVM.GroupId);
			if (group is null)
			{
				ModelState.AddModelError("GroupId", "Invalid choice");
				return View(studentVM);
			}
			GroupStudent found = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).FirstOrDefaultAsync(gs => gs.User.Id == studentVM.UserId && gs.Group.Id == studentVM.GroupId);
			if (found is not null)
			{
				ModelState.AddModelError("UserId", "This user has already have student role in selected group");
				return View(studentVM);
			}
			GroupStudent student = new()
			{
				User = user,
				Group = group,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.GroupStudents.AddAsync(student);
			await _context.Logs.AddAsync(new Log { Heading = "Added student", Text = $"{User.Identity.Name} added student.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(int id)
		{
			ViewBag.Page = "Students";
			GroupStudent student = await _context.GroupStudents.Include(gs => gs.User).Include(gs => gs.Group).FirstOrDefaultAsync(gs => gs.Id == id);
			if (student is null) return NotFound();
			StudentUpdateVM studentVM = new()
			{
				Id = student.Id,
				UserId = student.User.Id,
				GroupId = student.Group.Id
			};
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("student");
			return View(studentVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(StudentUpdateVM studentVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("student");
			GroupStudent student = await _context.GroupStudents.Include(gs => gs.User).Include(gs => gs.Group).FirstOrDefaultAsync(gs => gs.Id == studentVM.Id);
			if (student is null) return NotFound();
			if (!ModelState.IsValid) return View(studentVM);
			AppUser user = await _userManager.FindByIdAsync(studentVM.UserId);
			if (user is null)
			{
				ModelState.AddModelError("UserId", "Invalid choice");
				return View(studentVM);
			}
			Group group = await _context.Groups.FindAsync(studentVM.GroupId);
			if (group is null)
			{
				ModelState.AddModelError("GroupId", "Invalid choice");
				return View(studentVM);
			}
			GroupStudent found = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).FirstOrDefaultAsync(gs => gs.User.Id == studentVM.UserId && gs.Group.Id == studentVM.GroupId && gs.Id != studentVM.Id);
			if (found is not null)
			{
				ModelState.AddModelError("UserId", "This user has already have student role in selected group");
				return View(studentVM);
			}
			student.User = user;
			student.Group = group;
			student.UpdatedAt = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Updated student", Text = $"{User.Identity.Name} updated student [ID: {student.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(int id)
		{
			GroupStudent student = await _context.GroupStudents.FindAsync(id);
			if (student is null) return NotFound();
			_context.GroupStudents.Remove(student);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted student", Text = $"{User.Identity.Name} deleted student [ID: {student.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}
	}
}

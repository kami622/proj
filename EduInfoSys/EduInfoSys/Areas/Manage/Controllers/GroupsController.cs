using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class GroupsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public GroupsController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Groups";
			List<Group> groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			return View(groups);
		}
		public async Task<IActionResult> Create()
		{
			ViewBag.Page = "Groups";
			ViewBag.Departments = await _context.Departments.ToListAsync();
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Create(GroupCreateVM groupVM)
		{
			ViewBag.Departments = await _context.Departments.ToListAsync();
			if (!ModelState.IsValid) return View(groupVM);
			Department dep = await _context.Departments.FindAsync(groupVM.DepartmentId);
			if (dep is null)
			{
				ModelState.AddModelError("DepartmentId", "Incorrect choice");
				return View(groupVM);
			}
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Department.Id == dep.Id && g.Name == groupVM.Name);
			if (group is not null)
			{
				ModelState.AddModelError("Name", "Group with this name has already been created in selected department");
				return View(groupVM);
			}
			group = new Group
			{
				Name = groupVM.Name,
				Department = dep,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
			await _context.Groups.AddAsync(group);
			await _context.Logs.AddAsync(new Log { Heading = "Created group", Text = $"{User.Identity.Name} created new group.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(int id)
		{
			ViewBag.Page = "Groups";
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Id == id);
			if (group is null) return NotFound();
			GroupUpdateVM groupVM = new()
			{
				Id = group.Id,
				Name = group.Name,
				DepartmentId = group.Department.Id
			};
			ViewBag.Departments = await _context.Departments.ToListAsync();
			return View(groupVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(GroupUpdateVM groupVM)
		{
			ViewBag.Departments = await _context.Departments.ToListAsync();
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Id == groupVM.Id);
			if (group is null) return NotFound();
			if (!ModelState.IsValid) return View(groupVM);
			Department dep = await _context.Departments.FindAsync(groupVM.DepartmentId);
			if (dep is null)
			{
				ModelState.AddModelError("DepartmentId", "Incorrect choice");
				return View(groupVM);
			}
			Group found = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Department.Id == dep.Id && g.Name == groupVM.Name && g.Id != groupVM.Id);
			if (found is not null)
			{
				ModelState.AddModelError("Name", "Group with this name has already been created in selected department");
				return View(groupVM);
			}
			group.Name = groupVM.Name;
			group.Department = dep;
			group.UpdatedAt = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Updated group", Text = $"{User.Identity.Name} updated group [ID: {group.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(int id)
		{
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Id == id);
			if (group is null) return NotFound();
			_context.Groups.Remove(group);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted group", Text = $"{User.Identity.Name} deleted group [ID: {group.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return Ok();
		}

	}
}

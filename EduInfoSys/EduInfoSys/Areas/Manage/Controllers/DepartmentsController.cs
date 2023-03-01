using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class DepartmentsController : Controller
	{
		private readonly AppDbContext _context;
		public DepartmentsController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Departments";
			List<Department> departments = await _context.Departments.ToListAsync();
			return View(departments);
		}

		public IActionResult Create()
		{
			ViewBag.Page = "Departments";
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(DepartmentCreateVM depVM)
		{
			if (!ModelState.IsValid) return View(depVM);
			Department dep = await _context.Departments.FirstOrDefaultAsync(dp => dp.Name == depVM.Name);
			if (dep is not null)
			{
				ModelState.AddModelError("Name", "Department with this name has already been created");
				return View(depVM);
			}
			dep = new()
			{
				Name = depVM.Name,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.Departments.AddAsync(dep);
			await _context.Logs.AddAsync(new Log { Heading = "Created department", Text = $"{User.Identity.Name} created new department.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(int id)
		{
			ViewBag.Page = "Departments";
			Department found = await _context.Departments.FindAsync(id);
			if (found is null) return NotFound();
			DepartmentUpdateVM depVM = new()
			{
				Id = found.Id,
				Name = found.Name
			};
			return View(depVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(DepartmentUpdateVM depVM)
		{
			Department found = await _context.Departments.FindAsync(depVM.Id);
			if (found is null) return NotFound();
			if (!ModelState.IsValid) return View(depVM);
			Department dep = await _context.Departments.FirstOrDefaultAsync(dp => dp.Name == depVM.Name && dp.Id != depVM.Id);
			if (dep is not null)
			{
				ModelState.AddModelError("Name", "Department with this name has already been created");
				return View(depVM);
			}
			found.Name = depVM.Name;
			found.UpdatedAt = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Updated department", Text = $"{User.Identity.Name} updated department [ID: {found.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(int id)
		{
			Department found = await _context.Departments.FindAsync(id);
			if (found is null) return NotFound();
			_context.Departments.Remove(found);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted department", Text = $"{User.Identity.Name} deleted department [ID: {found.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

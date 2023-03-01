using EduInfoSys.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher")]
	public class TypeController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public TypeController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Create(int id)
		{
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject is null) return NotFound();
			TypeCreateVM typeVM = new()
			{
				Id = id
			};
			return View(typeVM);
		}

		public async Task<IActionResult> ShowTypes(int id)
		{
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject is null) return NotFound();
			List<TaskType> types = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == subject.Id).ToListAsync();
			ViewBag.Id = id;
			return PartialView("_TaskTypesPartial", types);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TypeCreateVM typeVM)
		{
			Subject subject = await _context.Subjects.FindAsync(typeVM.Id);
			if (subject is null) return NotFound();
			if (!ModelState.IsValid) return View(typeVM);
			List<TaskType> types = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == subject.Id).ToListAsync();
			if (types.Count > 9)
			{
				ModelState.AddModelError("", "Maximum count of task types is 10");
				return View(typeVM);
			}
			if (types.Sum(tt => tt.Percentage) + typeVM.Percentage > 100)
			{
				ModelState.AddModelError("", "Overall percentage must be less than 100%");
				return View(typeVM);
			}
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Name == typeVM.Name && tt.Subject.Id == subject.Id);
			if (type is not null)
			{
				ModelState.AddModelError("Name", "Type with this name has already been created");
				return View(typeVM);
			}
			type = new()
			{
				Name = typeVM.Name,
				Percentage = typeVM.Percentage,
				Subject = subject,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
			};
			await _context.TaskTypes.AddAsync(type);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = subject.Id, page = "settings", control = "tasktypes" });
		}

		public async Task<IActionResult> Update(int id)
		{
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == id);
			if (type is null) return NotFound();
			TypeCreateVM typeVM = new()
			{
				Id = id,
				Name = type.Name,
				Percentage = type.Percentage
			};
			ViewBag.Id = type.Subject.Id;
			return View(typeVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(TypeCreateVM typeVM)
		{
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == typeVM.Id);
			if (type is null) return NotFound();
			ViewBag.Id = type.Subject.Id;
			if (!ModelState.IsValid) return View(typeVM);
			List<TaskType> types = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == type.Subject.Id && tt.Id != type.Id).ToListAsync();
			if (types.Sum(tt => tt.Percentage) + typeVM.Percentage > 100)
			{
				ModelState.AddModelError("", "Overall percentage must be less than 100%");
				return View(typeVM);
			}
			if (typeVM.Name != type.Name)
			{
				TaskType check = await _context.TaskTypes.FirstOrDefaultAsync(tt => tt.Name == typeVM.Name && tt.Id != typeVM.Id);
				if (check is not null)
				{
					ModelState.AddModelError("Name", "Type with this name has already been created");
					return View(typeVM);
				}
			}
			type.Name = typeVM.Name;
			type.Percentage = typeVM.Percentage;
			type.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = type.Subject.Id, page = "settings", control = "tasktypes" });
		}

		public async Task<IActionResult> Delete(int id)
		{
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == id);
			if (type is null) return NotFound();
			_context.TaskTypes.Remove(type);
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

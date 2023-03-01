using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class SubjectsController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public SubjectsController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Subjects";
			List<Subject> subjects = await _context.Subjects.Include(s => s.Group).Include(s => s.Group.Department).ToListAsync();
			return View(subjects);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Page = "Subjects";
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(SubjectCreateVM subjectVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			if (!ModelState.IsValid) return View(subjectVM);
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Id == subjectVM.GroupId);
			if (group is null)
			{
				ModelState.AddModelError("GroupId", "Invalid choice");
				return View(subjectVM);
			}
			if (_context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == group.Id).Count() > 19)
			{
				ModelState.AddModelError("GroupId", "Maximum count of subjects in one group is 19");
				return View(subjectVM);
			}
			Subject found = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Name == subjectVM.Name && s.Group.Id == group.Id);
			if (found is not null)
			{
				ModelState.AddModelError("Name", "Subject with this name has already been created in this group");
				return View(subjectVM);
			}
			Subject subject = new()
			{
				Name = subjectVM.Name,
				Group = group,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.Subjects.AddAsync(subject);
			await _context.Logs.AddAsync(new Log { Heading = "Created subject", Text = $"{User.Identity.Name} created subject.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(int id)
		{
			ViewBag.Page = "Subjects";
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			Subject subject = await _context.Subjects.Include(g => g.Group).FirstOrDefaultAsync(g => g.Id == id);
			if (subject is null) return NotFound();
			SubjectUpdateVM subjectVM = new()
			{
				Name = subject.Name,
				GroupId = subject.Group.Id
			};
			return View(subjectVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(SubjectUpdateVM subjectVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			Subject subject = await _context.Subjects.Include(g => g.Group).FirstOrDefaultAsync(s => s.Id == subjectVM.Id);
			if (subject is null) return NotFound();
			if (!ModelState.IsValid) return View(subjectVM);
			Group group = await _context.Groups.Include(g => g.Department).FirstOrDefaultAsync(g => g.Id == subjectVM.GroupId);
			if (group is null)
			{
				ModelState.AddModelError("GroupId", "Invalid choice");
				return View(subjectVM);
			}
			Subject found = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Name == subjectVM.Name && s.Group.Id == subjectVM.GroupId && s.Id != subjectVM.Id);
			if (found is not null)
			{
				ModelState.AddModelError("Name", "Subject with this name has already been created in this group");
				return View(subjectVM);
			}
			subject.Name = subjectVM.Name;
			subject.Group = group;
			subject.UpdatedAt = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Updated subject", Text = $"{User.Identity.Name} updated subject [ID: {subject.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(int id)
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
			if (subject is null) return NotFound();
			_context.Subjects.Remove(subject);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted subject", Text = $"{User.Identity.Name} deleted subject [ID: {subject.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}
	}
}

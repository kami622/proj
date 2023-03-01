using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher")]
	public class SourceController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public SourceController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Create(int id)
		{
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			return View(new SourceCreateVM { Id = id });
		}

		[HttpPost]
		public async Task<IActionResult> Create(SourceCreateVM sourceVM)
		{
			Subject subject = await _context.Subjects.FindAsync(sourceVM.Id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			if (!ModelState.IsValid) return View(sourceVM);
			SubjectSource source = new()
			{
				Name = sourceVM.Name,
				Url = sourceVM.Url,
				Teacher = teacher,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.SubjectSources.AddAsync(source);
			await _context.SaveChangesAsync();
			return RedirectToAction("index", "subject", new { id = subject.Id });
		}

		public async Task<IActionResult> Update(int id)
		{
			SubjectSource source = await _context.SubjectSources.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == id);
			if (source is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == source.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			SourceCreateVM sourceVM = new()
			{
				Id = source.Id,
				Name = source.Name,
				Url = source.Url
			};
			ViewBag.SubjectId = teacher.Subject.Id;
			return View(sourceVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(SourceCreateVM sourceVM)
		{
			SubjectSource source = await _context.SubjectSources.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == sourceVM.Id);
			if (source is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == source.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			ViewBag.SubjectId = teacher.Subject.Id;
			if (!ModelState.IsValid) return View(sourceVM);
			source.Name = sourceVM.Name;
			source.Url = sourceVM.Url;
			source.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return RedirectToAction("index", "subject", new { id = source.Teacher.Subject.Id });
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectSource source = await _context.SubjectSources.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == id);
			if (source is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == source.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			_context.SubjectSources.Remove(source);
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher")]
	public class NoteController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public NoteController(AppDbContext context, UserManager<AppUser> userManager)
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
			return View(new NoteCreateVM { Id = id });
		}

		[HttpPost]
		public async Task<IActionResult> Create(NoteCreateVM noteVM)
		{
			Subject subject = await _context.Subjects.FindAsync(noteVM.Id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			if (!ModelState.IsValid) return View(noteVM);
			SubjectNote note = new()
			{
				Title = noteVM.Title,
				Description = noteVM.Description,
				Teacher = teacher,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.SubjectNotes.AddAsync(note);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = subject.Id });
		}

		public async Task<IActionResult> Update(int id)
		{
			SubjectNote note = await _context.SubjectNotes.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == id);
			if (note is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == note.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			NoteCreateVM noteVM = new()
			{
				Id = note.Id,
				Title = note.Title,
				Description = note.Description
			};
			ViewBag.SubjectId = teacher.Subject.Id;
			return View(noteVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(NoteCreateVM noteVM)
		{
			SubjectNote note = await _context.SubjectNotes.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == noteVM.Id);
			if (note is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == note.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			ViewBag.SubjectId = teacher.Subject.Id;
			if (!ModelState.IsValid) return View(noteVM);
			note.Title = noteVM.Title;
			note.Description = noteVM.Description;
			note.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = note.Teacher.Subject.Id });
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectNote note = await _context.SubjectNotes.Include(sn => sn.Teacher).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.User).FirstOrDefaultAsync(sn => sn.Id == id);
			if (note is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == note.Teacher.Subject.Id);
			if (teacher is null) return NotFound();
			_context.SubjectNotes.Remove(note);
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

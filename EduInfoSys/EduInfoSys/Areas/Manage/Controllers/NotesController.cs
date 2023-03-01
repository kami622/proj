using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class NotesController : Controller
	{
		private readonly AppDbContext _context;
		public NotesController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			List<SubjectNote> notes = await _context.SubjectNotes.Include(sn => sn.Teacher).Include(sn => sn.Teacher.User).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.Subject.Group).ToListAsync();
			return View(notes);
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectNote note = await _context.SubjectNotes.FindAsync(id);
			if (note is null) return NotFound();
			_context.SubjectNotes.Remove(note);
			await _context.Logs.AddAsync(new() { Heading = "Deleted note", Text = $"{User.Identity.Name} deleted note. [ID: {note.Id}]" });
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class SourcesController : Controller
	{
		private readonly AppDbContext _context;
		public SourcesController(AppDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			List<SubjectSource> sources = await _context.SubjectSources.Include(sn => sn.Teacher).Include(sn => sn.Teacher.User).Include(sn => sn.Teacher.Subject).Include(sn => sn.Teacher.Subject.Group).ToListAsync();
			return View(sources);
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectSource source = await _context.SubjectSources.FindAsync(id);
			if (source is null) return NotFound();
			_context.SubjectSources.Remove(source);
			await _context.Logs.AddAsync(new() { Heading = "Deleted source", Text = $"{User.Identity.Name} deleted source. [ID: {source.Id}]" });
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

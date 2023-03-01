using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class TasksController : Controller
	{
		private readonly AppDbContext _context;
		public TasksController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<SubjectTask> tasks = await _context.Tasks.Include(sn => sn.Type).Include(sn => sn.Type.Subject).Include(sn => sn.Type.Subject.Group).ToListAsync();
			return View(tasks);
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectTask task = await _context.Tasks.FindAsync(id);
			if (task is null) return NotFound();
			_context.Tasks.Remove(task);
			await _context.Logs.AddAsync(new() { Heading = "Deleted task", Text = $"{User.Identity.Name} deleted task. [ID: {task.Id}]" });
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

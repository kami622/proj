using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class TypesController : Controller
	{
		private readonly AppDbContext _context;
		public TypesController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index()
		{
			List<TaskType> types = await _context.TaskTypes.Include(sn => sn.Subject).Include(sn => sn.Subject.Group).ToListAsync();
			return View(types);
		}

		public async Task<IActionResult> Delete(int id)
		{
			TaskType type = await _context.TaskTypes.FindAsync(id);
			if (type is null) return NotFound();
			_context.TaskTypes.Remove(type);
			await _context.Logs.AddAsync(new() { Heading = "Deleted task type", Text = $"{User.Identity.Name} deleted task type. [ID: {type.Id}]" });
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

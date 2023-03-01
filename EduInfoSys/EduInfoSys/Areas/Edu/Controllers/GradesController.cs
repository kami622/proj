using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "student")]
	public class GradesController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public GradesController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			List<GroupStudent> groupStuds = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.Group.Department).Where(gs => gs.User.UserName == User.Identity.Name).ToListAsync();
			List<Group> groups = new();
			List<Subject> subjects = new();
			List<TaskType> taskTypes = new();
			List<TaskGrade> grades = new();
			foreach (GroupStudent groupStud in groupStuds)
			{
				groups.Add(groupStud.Group);
				subjects.AddRange(await _context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == groupStud.Group.Id).ToListAsync());
			}
			foreach (Subject subject in subjects)
			{
				taskTypes.AddRange(await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == subject.Id).ToListAsync());
				grades.AddRange(await _context.TaskGrades.Include(tg => tg.Task).Include(tg => tg.User).Include(tg => tg.Task.Type).Include(tg => tg.Task.Type.Subject).Where(tg => tg.Task.Type.Subject.Id == subject.Id).ToListAsync());
			}
			ViewBag.Groups = groups;
			ViewBag.Subjects = subjects;
			ViewBag.TaskTypes = taskTypes;
			ViewBag.Grades = grades;
			return View();
		}
	}
}

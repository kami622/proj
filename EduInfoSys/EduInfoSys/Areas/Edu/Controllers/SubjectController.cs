using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "student,teacher")]
	public class SubjectController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public SubjectController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public async Task<IActionResult> Details(int id, string page = "info", string control = "attendance")
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.Subject).Include(st => st.User).FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			GroupStudent student = await _context.GroupStudents.Include(st => st.Group).Include(st => st.User).FirstOrDefaultAsync(gs => gs.Group.Id == subject.Group.Id && gs.User.UserName == User.Identity.Name);
			string role = string.Empty;
			if (teacher is not null)
			{
				role = "Teacher";
			}
			else if (student is not null)
			{
				role = "Student";
			}
			else return NotFound();
			List<SubjectTask> tasks = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).Where(t => t.Type.Subject.Id == subject.Id).OrderByDescending(t => t.UpdatedAt).ToListAsync();
			List<TaskVM> taskVMs = new();
			foreach (SubjectTask task in tasks)
			{
				TaskVM taskVM = new()
				{
					SubjectTask = task,
					StudentTask = await _context.StudentTasks.FirstOrDefaultAsync(st => st.Task.Id == task.Id && st.Student.UserName == User.Identity.Name),
					TaskGrade = await _context.TaskGrades.FirstOrDefaultAsync(tg => tg.Task.Id == task.Id && tg.User.UserName == User.Identity.Name)
				};
				taskVMs.Add(taskVM);
			}
			SubjectVM subjectVM = new()
			{
				Subject = subject,
				Notes = await _context.SubjectNotes.Include(sn => sn.Teacher).Where(sn => sn.Teacher.Subject.Id == subject.Id).ToListAsync(),
				Sources = await _context.SubjectSources.Where(sn => sn.Teacher.Subject.Id == subject.Id).ToListAsync(),
				TaskVMs = taskVMs,
				Students = await _context.GroupStudents.Include(gs => gs.User).Where(gs => gs.Group.Id == subject.Group.Id).ToListAsync(),
				MemberRole = role
			};
			ViewBag.Page = page;
			ViewBag.Control = control;
			return View(subjectVM);
		}
	}
}

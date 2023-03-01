using EduInfoSys.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher, student")]
	public class TaskController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		private readonly IWebHostEnvironment _env;
		public TaskController(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
		{
			_context = context;
			_userManager = userManager;
			_env = env;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Details(int id)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).Include(t => t.Type.Subject.Group).FirstOrDefaultAsync(t => t.Id == id);
			if (task is null) return NotFound();
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (await _userManager.IsInRoleAsync(user, "teacher"))
			{
				if (!await _context.SubjectTeachers.AnyAsync(st => st.Subject.Id == task.Type.Subject.Id)) return NotFound();
				ViewBag.Role = "teacher";
			}
			if (await _userManager.IsInRoleAsync(user, "student"))
			{
				if (!await _context.GroupStudents.AnyAsync(st => st.Group.Id == task.Type.Subject.Group.Id)) return NotFound();
				ViewBag.Role = "student";
			}
			TaskVM taskVM = new()
			{
				SubjectTask = task,
				StudentTask = await _context.StudentTasks.FirstOrDefaultAsync(st => st.Task.Id == task.Id && st.Student.UserName == User.Identity.Name),
				TaskGrade = await _context.TaskGrades.FirstOrDefaultAsync(tg => tg.Task.Id == task.Id && tg.User.UserName == User.Identity.Name)
			};
			return View(taskVM);
		}

		[Authorize(Roles = "teacher")]
		public async Task<IActionResult> Create(int id)
		{
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject == null) return NotFound();
			TaskCreateVM taskVM = new()
			{
				Id = id
			};
			ViewBag.TaskTypes = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == subject.Id).ToListAsync();
			return View(taskVM);
		}

		[HttpPost, Authorize(Roles = "teacher")]
		public async Task<IActionResult> Create(TaskCreateVM taskVM)
		{
			Subject subject = await _context.Subjects.FindAsync(taskVM.Id);
			if (subject == null) return NotFound();
			ViewBag.TaskTypes = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == subject.Id).ToListAsync();
			if (!ModelState.IsValid) return View(taskVM);
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == taskVM.TaskTypeId);
			if (type == null)
			{
				ModelState.AddModelError("TaskTypeId", "Incorrect task type");
				return View(taskVM);
			}
			if (taskVM.StartsAt <= DateTime.Now)
			{
				ModelState.AddModelError("StartsAt", "Starts at date is incorrect");
				return View(taskVM);
			}
			if (taskVM.EndsAt < taskVM.StartsAt)
			{
				ModelState.AddModelError("EndsAt", "Ends at date must be later than Starts at date");
				return View(taskVM);
			}
			SubjectTask task = new()
			{
				Name = taskVM.Name,
				Description = taskVM.Description,
				IsNeedsFile = taskVM.IsNeedsFile,
				Type = type,
				StartsAt = taskVM.StartsAt,
				EndsAt = taskVM.EndsAt,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.Tasks.AddAsync(task);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = subject.Id, page = "tasks" });
		}

		[Authorize(Roles = "teacher")]
		public async Task<IActionResult> Update(int id)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).FirstOrDefaultAsync(t => t.Id == id);
			if (task is null) return NotFound();
			ViewBag.Id = id;
			ViewBag.TaskTypes = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == task.Type.Subject.Id).ToListAsync();
			TaskCreateVM taskVM = new()
			{
				Id = task.Id,
				Name = task.Name,
				Description = task.Description,
				StartsAt = task.StartsAt,
				EndsAt = task.EndsAt,
				IsNeedsFile = task.IsNeedsFile,
				TaskTypeId = task.Type.Id
			};
			return View(taskVM);
		}

		[HttpPost, Authorize(Roles = "teacher")]
		public async Task<IActionResult> Update(TaskCreateVM taskVM)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).FirstOrDefaultAsync(t => t.Id == taskVM.Id);
			if (task is null) return NotFound();
			ViewBag.Id = task.Type.Subject.Id;
			ViewBag.TaskTypes = await _context.TaskTypes.Include(tt => tt.Subject).Where(tt => tt.Subject.Id == task.Type.Subject.Id).ToListAsync();
			if (!ModelState.IsValid) return View(taskVM);
			TaskType type = await _context.TaskTypes.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == taskVM.TaskTypeId);
			if (type is null)
			{
				ModelState.AddModelError("TaskTypeId", "Incorrect task type");
				return View(taskVM);
			}
			if (taskVM.StartsAt <= task.CreatedAt)
			{
				ModelState.AddModelError("StartsAt", "Starts at date is incorrect");
				return View(taskVM);
			}
			if (taskVM.EndsAt < taskVM.StartsAt)
			{
				ModelState.AddModelError("EndsAt", "Ends at date must be later than Starts at date");
				return View(taskVM);
			}
			task.Name = taskVM.Name;
			task.Description = taskVM.Description;
			task.IsNeedsFile = taskVM.IsNeedsFile;
			task.StartsAt = taskVM.StartsAt;
			task.EndsAt = taskVM.EndsAt;
			task.Type = type;
			task.UpdatedAt = DateTime.Now;
			await _context.SaveChangesAsync();
			return RedirectToAction("details", new { id = task.Id });
		}

		[HttpPost]
		public async Task<IActionResult> Details(TaskAttachVM attachVM)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).Include(t => t.Type.Subject.Group).FirstOrDefaultAsync(t => t.Id == attachVM.Id);
			if (task is null) return NotFound();
			if (!task.IsNeedsFile)
			{
				TempData["CustomError"] = "Task doesn't need file attachment";
				return RedirectToAction("details");
			}
			if (DateTime.Now < task.StartsAt)
			{
				TempData["CustomError"] = "Task time is not started. Files can't be attached.";
				return RedirectToAction("details");
			}
			if (DateTime.Now > task.EndsAt)
			{
				TempData["CustomError"] = "Task time is out. New files can't be attached.";
				return RedirectToAction("details");
			}
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			if (await _userManager.IsInRoleAsync(user, "teacher"))
			{
				if (!await _context.SubjectTeachers.AnyAsync(st => st.Subject.Id == task.Type.Subject.Id)) return NotFound();
				ViewBag.Role = "teacher";
			}
			if (await _userManager.IsInRoleAsync(user, "student"))
			{
				if (!await _context.GroupStudents.AnyAsync(st => st.Group.Id == task.Type.Subject.Group.Id)) return NotFound();
				ViewBag.Role = "student";
			}
			
			GroupStudent student = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).FirstOrDefaultAsync(gs => gs.User.Id == user.Id && gs.Group.Id == task.Type.Subject.Group.Id);
			if (student is null) return NotFound();
			StudentTask studTask = await _context.StudentTasks.Include(st => st.Student).Include(st => st.Task).FirstOrDefaultAsync(st => st.Task.Id == task.Id && st.Student.Id == user.Id);
			if (studTask is not null) return RedirectToAction("details");
			if (!ModelState.IsValid)
			{
				TempData["CustomError"] = "File is not attached or incorrect type";
				return RedirectToAction("details");
			}
			(bool check, string msg) result = FileHelper.Check(attachVM.FormFile);
			if (!result.check)
			{
				TempData["CustomError"] = result.msg;
				return RedirectToAction("details");
			}
			studTask = new()
			{
				Task = task,
				Student = user,
				FilePath = FileHelper.Save(_env.WebRootPath, "assets/files/student", attachVM.FormFile),
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.StudentTasks.AddAsync(studTask);
			await _context.SaveChangesAsync();
			return RedirectToAction("details");
		}

		public async Task<IActionResult> Attachments(int id)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).Include(t => t.Type.Subject.Group).FirstOrDefaultAsync(t => t.Id == id);
			if (task is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.Subject).Include(st => st.User).FirstOrDefaultAsync(st => st.Subject.Id == task.Type.Subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			List<GroupStudent> students = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).Where(gs => gs.Group.Id == task.Type.Subject.Group.Id).ToListAsync();
			ViewBag.Files = await _context.StudentTasks.Include(st => st.Student).Include(st => st.Task).Include(st => st.Task.Type).Include(st => st.Task.Type.Subject).Where(st => st.Task.Id == task.Id).ToListAsync();
			ViewBag.Task = task;
			ViewBag.Grades = await _context.TaskGrades.Include(tg => tg.Task).Include(tg => tg.User).Where(tg => tg.Task.Id == task.Id).ToListAsync();
			return View(students);
		}

		[HttpPost]
		public async Task<IActionResult> Attachments(AttachmentVM attachVM)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).Include(t => t.Type.Subject.Group).FirstOrDefaultAsync(t => t.Id == attachVM.Id);
			if (task is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.Subject).Include(st => st.User).FirstOrDefaultAsync(st => st.Subject.Id == task.Type.Subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			List<GroupStudent> students = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).Where(gs => gs.Group.Id == task.Type.Subject.Group.Id).ToListAsync();
			ViewBag.Files = await _context.StudentTasks.Include(st => st.Student).Include(st => st.Task).Include(st => st.Task.Type).Include(st => st.Task.Type.Subject).Where(st => st.Task.Id == task.Id).ToListAsync();
			ViewBag.Task = task;
			ViewBag.Grades = await _context.TaskGrades.Include(tg => tg.Task).Include(tg => tg.User).Where(tg => tg.Task.Id == task.Id).ToListAsync();
			if (!ModelState.IsValid) return View(students);
			for (int i = 0; i < attachVM.StudentIds.Count; i++)
			{
				if (!students.Any(st => st.User.Id == attachVM.StudentIds[i])) return View(students);
				if (attachVM.Points[i] > 100 || attachVM.Points[i] < 0) return View(students);
				TaskGrade grade = await _context.TaskGrades.Include(tg => tg.Task).Include(tg => tg.User).FirstOrDefaultAsync(tg => tg.User.Id == attachVM.StudentIds[i] && tg.Task.Id == task.Id);
				if (grade is null)
				{
					grade = new()
					{
						Task = task,
						User = await _userManager.FindByIdAsync(attachVM.StudentIds[i]),
						Point = attachVM.Points[i],
						CreatedAt = DateTime.Now,
						UpdatedAt = DateTime.Now
					};
					await _context.TaskGrades.AddAsync(grade);
				}
				else
				{
					grade.Task = task;
					grade.User = await _userManager.FindByIdAsync(attachVM.StudentIds[i]);
					grade.Point = attachVM.Points[i];
					grade.UpdatedAt = DateTime.Now;
				}
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("attachments", new { id = task.Id });
		}

		public async Task<IActionResult> DeleteAttachment(int id)
		{
			SubjectTask task = await _context.Tasks.Include(st => st.Type).Include(st => st.Type.Subject).Include(st => st.Type.Subject.Group).FirstOrDefaultAsync(st => st.Id == id);
			if (task is null) return NotFound();
			AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
			GroupStudent student = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).FirstOrDefaultAsync(gs => gs.User.Id == user.Id && gs.Group.Id == task.Type.Subject.Group.Id);
			if (student is null) return NotFound();
			StudentTask studTask = await _context.StudentTasks.Include(st => st.Task).Include(st => st.Student).FirstOrDefaultAsync(st => st.Task.Id == task.Id && st.Student.Id == user.Id);
			if (studTask is null) return NotFound();
			FileHelper.Delete(_env.WebRootPath, "assets/files/student", studTask.FilePath);
			_context.StudentTasks.Remove(studTask);
			await _context.SaveChangesAsync();
			return RedirectToAction("details");
		}

		[Authorize(Roles = "teacher")]
		public async Task<IActionResult> Delete(int id)
		{
			SubjectTask task = await _context.Tasks.Include(t => t.Type).Include(t => t.Type.Subject).FirstOrDefaultAsync(t => t.Id == id);
			if (task is null) return NotFound();
			_context.Tasks.Remove(task);
			List<StudentTask> studTasks = await _context.StudentTasks.Include(st => st.Task).Where(st => st.Task.Id == task.Id).ToListAsync();
			foreach (StudentTask studTask in studTasks)
			{
				FileHelper.Delete(_env.WebRootPath, "assets/files/student", studTask.FilePath);
			}
			_context.StudentTasks.RemoveRange(studTasks);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = task.Type.Subject.Id, page = "tasks" });
		}
	}
}

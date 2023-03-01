using Microsoft.AspNetCore.Mvc;
using NuGet.DependencyResolver;

namespace EduInfoSys.Areas.Manage.Controllers
{
	[Area("manage"), Authorize(Roles = "superadmin,admin")]
	public class TeachersController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public TeachersController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			ViewBag.Page = "Teachers";
			List<SubjectTeacher> teachers = await _context.SubjectTeachers.Include(t => t.User).Include(t => t.Subject).Include(t => t.Subject.Group).Include(t => t.Subject.Group.Department).ToListAsync();
			return View(teachers);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Page = "Teachers";
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("teacher");
			ViewBag.Subjects = new List<Subject>();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ShowSubjects([FromBody]int groupId)
		{
			List<Subject> subjects = await _context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == groupId).ToListAsync();
			return Json(subjects);
		}

		[HttpPost]
		public async Task<IActionResult> Create(TeacherCreateVM teacherVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("teacher");
			ViewBag.Subjects = await _context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == teacherVM.GroupId).ToListAsync();
			if (!ModelState.IsValid) return View(teacherVM);
			AppUser user = await _userManager.FindByIdAsync(teacherVM.UserId);
			if (user is null)
			{
				ModelState.AddModelError("UserId", "Invalid choice");
				return View(teacherVM);
			}
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == teacherVM.SubjectId);
			if (subject is null)
			{
				ModelState.AddModelError("SubjectId", "Invalid choice");
				return View(teacherVM);
			}
			SubjectTeacher subjectTeacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.Id == teacherVM.UserId && st.Subject.Id == teacherVM.SubjectId);
			if (subjectTeacher is not null)
			{
				ModelState.AddModelError("UserId", "This user has already have teacher role in selected subject");
				return View(teacherVM);
			}
			SubjectTeacher teacher = new()
			{
				User = user,
				Subject = subject,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};
			await _context.SubjectTeachers.AddAsync(teacher);
			await _context.Logs.AddAsync(new Log { Heading = "Added teacher", Text = $"{User.Identity.Name} added teacher.", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Update(int id)
		{
			ViewBag.Page = "Teachers";
			SubjectTeacher subjectTeacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).Include(st => st.Subject.Group).FirstOrDefaultAsync(st => st.Id == id);
			if (subjectTeacher is null) return NotFound();
			TeacherUpdateVM teacherVM = new()
			{
				Id = id,
				SubjectId = subjectTeacher.Subject.Id,
				GroupId = subjectTeacher.Subject.Group.Id,
				UserId = subjectTeacher.User.Id
			};
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("teacher");
			ViewBag.Subjects = await _context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == teacherVM.GroupId).ToListAsync();
			return View(teacherVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(TeacherUpdateVM teacherVM)
		{
			ViewBag.Groups = await _context.Groups.Include(g => g.Department).ToListAsync();
			ViewBag.Users = await _userManager.GetUsersInRoleAsync("teacher");
			ViewBag.Subjects = await _context.Subjects.Include(s => s.Group).Where(s => s.Group.Id == teacherVM.GroupId).ToListAsync();
			SubjectTeacher found = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.Id == teacherVM.Id);
			if (found is null) return NotFound();
			if (!ModelState.IsValid) return View(teacherVM);
			AppUser user = await _userManager.FindByIdAsync(teacherVM.UserId);
			if (user is null)
			{
				ModelState.AddModelError("UserId", "Invalid choice");
				return View(teacherVM);
			}
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == teacherVM.SubjectId);
			if (subject is null)
			{
				ModelState.AddModelError("SubjectId", "Invalid choice");
				return View(teacherVM);
			}
			SubjectTeacher subjectTeacher = await _context.SubjectTeachers.Include(st => st.User).Include(st => st.Subject).FirstOrDefaultAsync(st => st.User.Id == teacherVM.UserId && st.Subject.Id == teacherVM.SubjectId && st.Id != teacherVM.Id);
			if (subjectTeacher is not null)
			{
				ModelState.AddModelError("UserId", "This user has already have teacher role in selected subject");
				return View(teacherVM);
			}
			found.User = user;
			found.Subject = subject;
			found.UpdatedAt = DateTime.Now;
			await _context.Logs.AddAsync(new Log { Heading = "Updated teacher", Text = $"{User.Identity.Name} updated teacher [ID: {found.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectTeacher teacher = await _context.SubjectTeachers.FindAsync(id);
			if (teacher is null) return NotFound();
			_context.SubjectTeachers.Remove(teacher);
			await _context.Logs.AddAsync(new Log { Heading = "Deleted teacher", Text = $"{User.Identity.Name} deleted teacher [ID: {teacher.Id}].", Date = DateTime.Now });
			await _context.SaveChangesAsync();
			return RedirectToAction("index");
		}
	}
}


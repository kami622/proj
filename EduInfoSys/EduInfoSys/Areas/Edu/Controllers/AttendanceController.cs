using EduInfoSys.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher")]
	public class AttendanceController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public AttendanceController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index(int id)
		{
			ViewBag.Id = id;
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject == null) return NotFound();
			List<Attendance> attendances = await _context.Attendances.Include(at => at.Subject).Include(at => at.Subject.Group).Include(at => at.User).Where(a => a.Subject.Id == id).ToListAsync();
			return View(attendances);
		}
		
		public async Task<IActionResult> Create(int id)
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
			if (subject is null) return NotFound();
			List<GroupStudent> students = await _context.GroupStudents.Include(gs => gs.User).Include(gs => gs.Group).Where(gs => gs.Group.Id == subject.Group.Id).ToListAsync();
			List<AttendanceVM> attVMs = new();
			foreach (GroupStudent student in students)
			{
				attVMs.Add(new() { Student = student });
			}
			ViewBag.Students = attVMs;
			AttendanceCreateVM attCM = new() { Id = id };
			return View(attCM);
		}

		[HttpPost]
		public async Task<IActionResult> Create(AttendanceCreateVM attVM)
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == attVM.Id);
			if (subject is null) return NotFound();
			List<GroupStudent> students = await _context.GroupStudents.Include(gs => gs.User).Include(gs => gs.Group).Where(gs => gs.Group.Id == subject.Group.Id).ToListAsync();
			List<AttendanceVM> attVMs = new();
			foreach (GroupStudent student in students)
			{
				attVMs.Add(new() { Student = student });
			}
			ViewBag.Students = attVMs;
			if (!ModelState.IsValid) return View(attVM);
			if (await _context.Attendances.AnyAsync(at => at.Date.Date == attVM.Date.Date))
			{
				ModelState.AddModelError("", "Attendance with this date has already been marked");
				return View(attVM);
			}
			if (!attVM.Marks.Contains(1) && !attVM.Marks.Contains(0) && !attVM.Marks.Contains(2))
			{
				ModelState.AddModelError("", "Something went wrong");
				return View(attVM);
			}
			for (int i = 0; i < attVM.StudentIds.Count(); i++)
			{
				AppUser user = await _userManager.FindByIdAsync(attVM.StudentIds[i]);
				if (user is null)
				{
					ModelState.AddModelError("", "Something went wrong");
					return View(attVM);
				}
				await _context.Attendances.AddAsync(new() { Subject = subject, User = user, Date = attVM.Date, Mark = attVM.Marks[i], Comment = attVM.Comments[i], CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now });
			}
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = subject.Id, page = "settings", control="attendance" });
		}

		public async Task<IActionResult> SingleAdd()
		{
			return View();
		}

		public async Task<IActionResult> Update(int id)
		{
			Attendance att = await _context.Attendances.Include(at => at.User).Include(at => at.Subject).FirstOrDefaultAsync(at => at.Id == id);
			if (att is null) return NotFound();
			AttendanceUpdateVM attVM = new()
			{
				Id = att.Id,
				Student = att.User,
				Mark= att.Mark,
				Comment= att.Comment
			};
			return PartialView("_UpdateAttendancePartial", attVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update([FromBody]AttendanceUpdateVM attVM)
		{
			Attendance att = await _context.Attendances.Include(at => at.User).Include(at => at.Subject).FirstOrDefaultAsync(at => at.Id == attVM.Id);
			if (att is null) return Json("Attendance is not found");
			if (att.Mark < 0 && att.Mark > 2) return Json("Incorrect mark");
			if (att.Comment is not null && att.Comment.Length > 100) return Json("Comment length must be less than 100 characters");
			att.Mark = attVM.Mark;
			att.Comment = attVM.Comment;
			await _context.SaveChangesAsync();
			return Json("ok");
		}

		public async Task<IActionResult> Delete(int id)
		{
			Attendance att = await _context.Attendances.Include(a => a.Subject).FirstOrDefaultAsync(a => a.Id == id);
			if (att is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(t => t.User).Include(t => t.Subject).FirstOrDefaultAsync(t => t.User.UserName == User.Identity.Name && t.Subject.Id == att.Subject.Id);
			if (teacher is null) return NotFound();
			List<Attendance> atts = await _context.Attendances.Where(t => t.Date == att.Date && t.Subject.Id == att.Subject.Id).ToListAsync();
			if (atts.Count == 0) return NotFound();
			_context.Attendances.RemoveRange(atts);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = att.Subject.Id, page = "settings", control = "attendance" });
		}

		public async Task<IActionResult> ShowAttendance(int id)
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.Include(st => st.Subject).Include(st => st.User).FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			List<Attendance> attendances = await _context.Attendances.Include(at => at.Subject).Include(at => at.Subject.Group).Include(at => at.User).Where(a => a.Subject.Id == id).ToListAsync();
			ViewBag.Id = id;
			return PartialView("_AttendancePartial", attendances);
		}
	}
}

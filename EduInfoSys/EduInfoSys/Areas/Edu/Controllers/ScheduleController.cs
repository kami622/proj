using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Areas.Edu.Controllers
{
	[Area("edu"), Authorize(Roles = "teacher")]
	public class ScheduleController : Controller
	{
		private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;
		public ScheduleController(AppDbContext context, UserManager<AppUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Create(int id)
		{
			Subject subject = await _context.Subjects.FindAsync(id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.FirstOrDefaultAsync(st => st.Subject.Id == subject.Id && st.User.UserName == User.Identity.Name);
			if (teacher is null) return NotFound();
			ScheduleCreateVM scheduleVM = new()
			{
				Id = id
			};
			return View(scheduleVM);
		}

		public async Task<IActionResult> ShowSchedules(int id)
		{
			Subject subject = await _context.Subjects.Include(s => s.Group).FirstOrDefaultAsync(s => s.Id == id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Group.Id == subject.Group.Id);
			if (teacher is null) return NotFound();
			List<SubjectSchedule> schedules = await _context.SubjectSchedules.Include(ss => ss.Subject).Where(ss => ss.Subject.Group.Id == subject.Group.Id).OrderBy(ss => ss.WeekDay).OrderBy(ss => ss.StartTime).ToListAsync();
			ViewBag.Id = id;
			return PartialView("_SchedulesPartial", schedules);
		}

		[HttpPost]
		public async Task<IActionResult> Create(ScheduleCreateVM scheduleVM)
		{
			Subject subject = await _context.Subjects.FindAsync(scheduleVM.Id);
			if (subject is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == subject.Id);
			if (teacher is null) return NotFound();
			if (!ModelState.IsValid) return View(scheduleVM);
			if (scheduleVM.StartTime.TimeOfDay >= scheduleVM.EndTime.TimeOfDay)
			{
				ModelState.AddModelError("", "Start time can't be greater than end time");
				return View(scheduleVM);
			}
			SubjectSchedule schedule = await _context.SubjectSchedules.FirstOrDefaultAsync(st => st.WeekDay == scheduleVM.WeekDay && ((st.StartTime <= scheduleVM.StartTime && st.EndTime >= scheduleVM.StartTime) || (st.StartTime < scheduleVM.EndTime && st.EndTime > scheduleVM.EndTime)));
			if (schedule is not null)
			{
				ModelState.AddModelError("", "There are other subject in this interval");
				return View(scheduleVM);
			}
			schedule = new()
			{
				Subject = subject,
				WeekDay = scheduleVM.WeekDay,
				StartTime = scheduleVM.StartTime,
				EndTime = scheduleVM.EndTime,
			};
			await _context.SubjectSchedules.AddAsync(schedule);
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = subject.Id, page = "settings", control = "schedule" });
		}

		public async Task<IActionResult> Update(int id)
		{
			SubjectSchedule schedule = await _context.SubjectSchedules.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == id);
			if (schedule is null) return NotFound();
			ScheduleCreateVM scheduleVM = new()
			{
				Id = id,
				WeekDay = schedule.WeekDay,
				StartTime = schedule.StartTime,
				EndTime = schedule.EndTime,
			};
			ViewBag.Id = schedule.Subject.Id;
			return View(scheduleVM);
		}

		[HttpPost]
		public async Task<IActionResult> Update(ScheduleCreateVM scheduleVM)
		{
			SubjectSchedule schedule = await _context.SubjectSchedules.Include(tt => tt.Subject).Include(tt => tt.Subject.Group).FirstOrDefaultAsync(tt => tt.Id == scheduleVM.Id);
			if (schedule is null) return NotFound();
			ViewBag.Id = schedule.Subject.Id;
			SubjectTeacher teacher = await _context.SubjectTeachers.FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == schedule.Subject.Id);
			if (teacher is null) return NotFound();
			if (!ModelState.IsValid) return View(scheduleVM);
			if (scheduleVM.StartTime.TimeOfDay >= scheduleVM.EndTime.TimeOfDay)
			{
				ModelState.AddModelError("", "Start time can't be greater than end time");
				return View(scheduleVM);
			}
			SubjectSchedule found = await _context.SubjectSchedules.FirstOrDefaultAsync(st => st.Subject.Id == schedule.Subject.Id && ((st.StartTime < scheduleVM.StartTime && st.EndTime > scheduleVM.StartTime) || (st.StartTime < scheduleVM.EndTime && st.EndTime > scheduleVM.EndTime)));
			if (found is not null)
			{
				ModelState.AddModelError("", "There are other subject in this interval");
				return View(scheduleVM);
			}
			schedule.WeekDay = scheduleVM.WeekDay;
			schedule.StartTime = scheduleVM.StartTime;
			schedule.EndTime = scheduleVM.EndTime;
			await _context.SaveChangesAsync();
			return RedirectToAction("details", "subject", new { id = schedule.Subject.Id, page = "settings", control = "schedule" });
		}

		public async Task<IActionResult> Delete(int id)
		{
			SubjectSchedule schedule = await _context.SubjectSchedules.Include(tt => tt.Subject).FirstOrDefaultAsync(tt => tt.Id == id);
			if (schedule is null) return NotFound();
			SubjectTeacher teacher = await _context.SubjectTeachers.FirstOrDefaultAsync(st => st.User.UserName == User.Identity.Name && st.Subject.Id == schedule.Subject.Id);
			if (teacher is null) return NotFound();
			_context.SubjectSchedules.Remove(schedule);
			await _context.SaveChangesAsync();
			return Ok();
		}
	}
}

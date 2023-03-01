using Microsoft.AspNetCore.Mvc;

namespace EduInfoSys.Services
{
	public class LayoutService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly AppDbContext _context;
		private readonly IHttpContextAccessor _accessor;
		public LayoutService(UserManager<AppUser> userManager, IHttpContextAccessor accessor, AppDbContext context)
		{
			_userManager = userManager;
			_accessor = accessor;
			_context = context;
		}

		public async Task<UserVM> GetUserAsync()
		{
			AppUser user = await _userManager.FindByNameAsync(_accessor.HttpContext.User.Identity.Name);
			string role = string.Empty;
			if (await _userManager.IsInRoleAsync(user, "superadmin")) role = "Super Administrator";
			else if (await _userManager.IsInRoleAsync(user, "admin")) role = "Administrator";
			else if (await _userManager.IsInRoleAsync(user, "teacher")) role = "Teacher";
			else if (await _userManager.IsInRoleAsync(user, "student")) role = "Student";
			UserVM userVM = new()
			{
				User = user,
				Role = role
			};
			return userVM;
		}

		public async Task<List<Subject>> GetStudentGroupsAsync()
		{
			List<GroupStudent> studentGroups = await _context.GroupStudents.Include(gs => gs.Group).Include(gs => gs.User).Where(gs => gs.User.UserName == _accessor.HttpContext.User.Identity.Name).ToListAsync();
			List<Subject> subjects = new();
			foreach (GroupStudent studentGroup in studentGroups)
			{
				subjects.AddRange(await _context.Subjects.Include(s => s.Group).Include(s => s.Group.Department).Where(s => s.Group.Id == studentGroup.Group.Id).ToListAsync());
			}
			return subjects;
		}

		public async Task<List<Subject>> GetTeacherSubjectsAsync()
		{
			List<SubjectTeacher> subjectTeachers = await _context.SubjectTeachers.Include(gt => gt.User).Include(gt => gt.Subject).Include(gt => gt.Subject.Group).Include(gt => gt.Subject.Group.Department).Where(gt => gt.User.UserName == _accessor.HttpContext.User.Identity.Name).ToListAsync();
			List<Subject> subjects = new();
			foreach (SubjectTeacher subjectTeacher in subjectTeachers)
			{
				subjects.Add(subjectTeacher.Subject);
			}
			return subjects;
		}

		public async Task<Customization> GetCustomizationAsync()
		{
			AppUser user = await _userManager.FindByNameAsync(_accessor.HttpContext.User.Identity.Name);
			Customization custom = null;
			if (user is not null)
			{
				custom = await _context.Customizations.FirstOrDefaultAsync(c => c.User.Id == user.Id);
			}
			if (custom is null)
			{
				custom = new()
				{
					ColorScheme = "default",
					SidebarLayout = "default",
					SidebarPosition = "left",
					Layout = "fluid"
				};
			}
			return custom;
		}
	}
}

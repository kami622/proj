namespace EduInfoSys.DAL
{
	public class AppDbContext : IdentityDbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<AppUser> Users { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<GroupStudent> GroupStudents { get; set; }
		public DbSet<Subject> Subjects { get; set; }
		public DbSet<SubjectTeacher> SubjectTeachers { get; set; }
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<SubjectNote> SubjectNotes { get; set; }
		public DbSet<SubjectSource> SubjectSources { get; set; }
		public DbSet<TaskType> TaskTypes { get; set; }
		public DbSet<SubjectTask> Tasks { get; set; }
		public DbSet<StudentTask> StudentTasks { get; set; }
		public DbSet<Log> Logs { get; set; }
		public DbSet<Customization> Customizations { get; set; }
		public DbSet<TaskGrade> TaskGrades { get; set; }
		public DbSet<SubjectSchedule> SubjectSchedules { get; set; }
		public DbSet<Notification> Notifications { get; set; }
	}
}

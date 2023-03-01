namespace EduInfoSys.Models
{
	public class StudentTask : BaseEntity
	{
		public SubjectTask Task { get; set; }
		public AppUser Student { get; set; }
		public string FilePath { get; set; }
	}
}

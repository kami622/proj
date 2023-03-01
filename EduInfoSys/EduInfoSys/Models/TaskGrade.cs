namespace EduInfoSys.Models
{
	public class TaskGrade : BaseEntity
	{
		public SubjectTask Task { get; set; }
		public AppUser User { get; set; }
		public double Point { get; set; }
	}
}

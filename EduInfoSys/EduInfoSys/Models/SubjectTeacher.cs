namespace EduInfoSys.Models
{
	public class SubjectTeacher : BaseEntity
	{
		public Subject Subject { get; set; }
		public AppUser User { get; set; }
	}
}

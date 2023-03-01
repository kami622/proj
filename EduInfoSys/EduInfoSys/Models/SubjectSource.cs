namespace EduInfoSys.Models
{
	public class SubjectSource : BaseEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
		[MaxLength(255)]
		public string Url { get; set; }
		public SubjectTeacher Teacher { get; set; }
	}
}

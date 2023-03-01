namespace EduInfoSys.Models
{
	public class SubjectNote : BaseEntity
	{
		[MaxLength(100)]
		public string Title { get; set; }
		[MaxLength(1500)]
		public string Description { get; set; }
		public SubjectTeacher Teacher { get; set; }
	}
}

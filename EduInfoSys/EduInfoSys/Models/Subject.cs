namespace EduInfoSys.Models
{
	public class Subject : BaseEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
		public Group Group { get; set; }
	}
}

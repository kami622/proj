namespace EduInfoSys.Models
{
	public class Department : BaseEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
	}
}

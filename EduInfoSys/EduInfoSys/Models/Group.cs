namespace EduInfoSys.Models
{
	public class Group : BaseEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
		public Department Department { get; set; }
	}
}

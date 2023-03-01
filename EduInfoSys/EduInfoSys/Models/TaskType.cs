namespace EduInfoSys.Models
{
	public class TaskType : BaseEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
		[Range(0, 100)]
		public double Percentage { get; set; }
		public Subject Subject { get; set; }
	}
}

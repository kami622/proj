namespace EduInfoSys.Models
{
	public class SubjectTask : BaseEntity
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsNeedsFile { get; set; }
		public TaskType Type { get; set; }
		public DateTime StartsAt { get; set; }
		public DateTime EndsAt { get; set; }
	}
}

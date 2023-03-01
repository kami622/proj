namespace EduInfoSys.Models
{
	public class SubjectSchedule : BaseEntity
	{
		public Subject Subject { get; set; }
		public int WeekDay { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}

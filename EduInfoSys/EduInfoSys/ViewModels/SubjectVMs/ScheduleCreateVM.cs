namespace EduInfoSys.ViewModels.SubjectVMs
{
	public class ScheduleCreateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Day of week is required"), Range(1, 7, ErrorMessage = "Day of week is incorrect")]
		public int WeekDay { get; set; }
		[Required(ErrorMessage = "Start time is required")]
		public DateTime StartTime { get; set; }
		[Required(ErrorMessage = "End time is required")]
		public DateTime EndTime { get; set; }
	}
}

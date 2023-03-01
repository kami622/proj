namespace EduInfoSys.ViewModels.TaskVMs
{
	public class TaskCreateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name field is required"), MaxLength(60, ErrorMessage = "Name length must be less than 60 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Description is required"), MaxLength(500, ErrorMessage = "Description length must be less than 500 characters")]
		public string Description { get; set; }
		public bool IsNeedsFile { get; set; }
		[Required(ErrorMessage = "Task type is required")]
		public int TaskTypeId { get; set; }
		[Required(ErrorMessage = "Starting date is required"), DataType(DataType.DateTime)]
		public DateTime StartsAt { get; set; }
		[Required(ErrorMessage = "Ending date is required"), DataType(DataType.DateTime)]
		public DateTime EndsAt { get; set; }
	}
}

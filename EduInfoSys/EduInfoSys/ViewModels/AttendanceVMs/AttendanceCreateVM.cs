namespace EduInfoSys.ViewModels.AttendanceVMs
{
	public class AttendanceCreateVM
	{
		public int Id { get; set; }
		[DataType(DataType.Date)]
		public DateTime Date { get; set; }
		[Required(ErrorMessage = "Student is required")]
		public List<string> StudentIds { get; set; }
		[Required(ErrorMessage = "Mark is required")]
		public List<int> Marks { get; set; }
		public List<string> Comments { get; set; }
	}
}

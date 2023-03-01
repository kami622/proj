namespace EduInfoSys.ViewModels.AttendanceVMs
{
	public class AttendanceUpdateVM
	{
		public int Id { get; set; }
		public AppUser? Student { get; set; }
		[Required(ErrorMessage = "Mark is required"), Range(0, 2, ErrorMessage = "Invalid mark")]
		public int Mark { get; set; }
		[MaxLength(100, ErrorMessage = "Comment length must be less than 100 characters")]
		public string? Comment { get; set; }
	}
}

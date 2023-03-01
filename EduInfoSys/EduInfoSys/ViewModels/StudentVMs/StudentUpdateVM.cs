namespace EduInfoSys.ViewModels.StudentVMs
{
	public class StudentUpdateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Group is required")]
		public int GroupId { get; set; }
		[Required(ErrorMessage = "User is required")]
		public string UserId { get; set; }
	}
}

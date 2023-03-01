namespace EduInfoSys.ViewModels.StudentVMs
{
	public class StudentCreateVM
	{
		[Required(ErrorMessage = "Group is required")]
		public int GroupId { get; set; }
		[Required(ErrorMessage = "User is required")]
		public string UserId { get; set; }
	}
}

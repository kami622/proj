namespace EduInfoSys.ViewModels.TeacherVMs
{
	public class TeacherCreateVM
	{
		[Required(ErrorMessage = "Group is required")]
		public int GroupId { get; set; }
		[Required(ErrorMessage = "Subject is required")]
		public int SubjectId { get; set; }
		[Required(ErrorMessage = "User is required")]
		public string UserId { get; set; }
	}
}

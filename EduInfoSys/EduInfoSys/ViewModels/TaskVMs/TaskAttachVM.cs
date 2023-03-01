namespace EduInfoSys.ViewModels.TaskVMs
{
	public class TaskAttachVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "File is required")]
		public IFormFile FormFile { get; set; }
	}
}

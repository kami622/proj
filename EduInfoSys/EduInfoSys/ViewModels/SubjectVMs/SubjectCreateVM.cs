namespace EduInfoSys.ViewModels.SubjectVMs
{
	public class SubjectCreateVM
	{
		[Required(ErrorMessage = "Name field is required"), MaxLength(100, ErrorMessage = "Name length must be less than 100 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Group is required")]
		public int GroupId { get; set; }
	}
}

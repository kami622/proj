namespace EduInfoSys.ViewModels.SubjectVMs
{
	public class SubjectUpdateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name field is required"), MaxLength(100, ErrorMessage = "Name length must be less than 100 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Group is required")]
		public int GroupId { get; set; }
	}
}

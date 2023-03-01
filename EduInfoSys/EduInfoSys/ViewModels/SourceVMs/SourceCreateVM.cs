namespace EduInfoSys.ViewModels.SourceVMs
{
	public class SourceCreateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name field is required"), MaxLength(100, ErrorMessage = "Name length must be less than 100 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "URL field is required"), MaxLength(255, ErrorMessage = "URL length must be less than 255 characters")]
		public string Url { get; set; }
	}
}

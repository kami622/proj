namespace EduInfoSys.ViewModels.NoteVMs
{
	public class NoteCreateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Title field is required"), MaxLength(100, ErrorMessage = "Title length must be less than 100 characters")]
		public string Title { get; set; }
		[Required(ErrorMessage = "Description field is required"), MaxLength(1500, ErrorMessage = "Too many characters in description")]
		public string Description { get; set; }
	}
}

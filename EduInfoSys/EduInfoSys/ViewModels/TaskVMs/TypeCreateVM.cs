namespace EduInfoSys.ViewModels.TaskVMs
{
	public class TypeCreateVM
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Name field is required"), MaxLength(50, ErrorMessage = "Name length must be less than 50 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Percentage field is required"), Range(0, 100, ErrorMessage = "Percentage range is 0-100")]
		public double Percentage { get; set; }
	}
}

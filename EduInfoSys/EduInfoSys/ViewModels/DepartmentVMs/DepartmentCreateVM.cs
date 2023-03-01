namespace EduInfoSys.ViewModels.DepartmentVMs
{
	public class DepartmentCreateVM
	{
		[Required(ErrorMessage = "Name field is required"), MaxLength(50, ErrorMessage = "Name length must be less than 50 characters")]
		public string Name { get; set; }
	}
}

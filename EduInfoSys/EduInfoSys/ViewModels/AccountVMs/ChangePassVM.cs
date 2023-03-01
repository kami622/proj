namespace EduInfoSys.ViewModels.AccountVMs
{
	public class ChangePassVM
	{
		[Required(ErrorMessage = "Current password field is required"), DataType(DataType.Password)]
		public string CurrentPassword { get; set; }
		[Required(ErrorMessage = "New password field is required"), StringLength(20, MinimumLength = 8, ErrorMessage = "Password length must be more than 8 and less than 20 characters"), DataType(DataType.Password)]
		public string NewPassword { get; set; }
		[Required(ErrorMessage = "Confirm password field is required"), Compare(nameof(NewPassword), ErrorMessage = "Passwords don't match"), DataType(DataType.Password)]
		public string ConfirmPassword { get; set;}
	}
}

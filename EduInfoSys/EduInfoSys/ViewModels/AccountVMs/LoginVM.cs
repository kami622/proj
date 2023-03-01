namespace EduInfoSys.ViewModels.AccountVMs
{
	public class LoginVM
	{
		[Required(ErrorMessage = "Username field is required")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Password field is required"),DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
}

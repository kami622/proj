namespace EduInfoSys.ViewModels.AccountVMs
{
	public class UserCreateVM
	{
		[Required(ErrorMessage = "Username field is required"), StringLength(18, MinimumLength = 3, ErrorMessage = "Username length must be more than 3 and less than 18")]
		public string UserName { get; set; }
		[Required(ErrorMessage = "Role is required")]
		public string Role { get; set; }
		[Required(ErrorMessage = "First name field is required"), MaxLength(30, ErrorMessage = "First name length must be less than 30 characters")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Last name field is required"), MaxLength(30, ErrorMessage = "Last name length must be less than 30 characters")]
		public string LastName { get; set; }
		[MaxLength(50, ErrorMessage = "Email length must be less than 50 characters"), DataType(DataType.EmailAddress)]
		public string? Email { get; set; }
		[MaxLength(20, ErrorMessage = "Phone number length must be less than 20 characters")]
		public string? PhoneNumber { get; set; }
		[Required(ErrorMessage = "Gender is required")]
		public string Gender { get; set; }
		[MaxLength(300, ErrorMessage = "Additional information length must be less than 300 characters")]
		public string? AdditionalInfo { get; set; }
		[Required(ErrorMessage = "Password field is required"), StringLength(20, MinimumLength = 8, ErrorMessage = "Password length must be more than 8 and less than 20 characters"), DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm password field is required"), DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords don't match")]
		public string ConfirmPassword { get; set; }
		[Required(ErrorMessage = "Birth date is required"), Range(typeof(DateTime), "1/1/1920", "1/1/2020", ErrorMessage = "Birth date is incorrect"), DataType(DataType.Date)]
		public DateTime BirthDate { get; set; }
		public IFormFile? ImageFile { get; set; }

	}
}

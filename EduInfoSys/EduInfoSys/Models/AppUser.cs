namespace EduInfoSys.Models
{
	public class AppUser : IdentityUser
	{
		[MaxLength(100)]
		public string FirstName { get; set; }
		[MaxLength(100)]
		public string LastName { get; set; }
		public string PhotoPath { get; set; }
		public DateTime BirthDate { get; set; }
		[MaxLength(300)]
		public string AdditionalInfo { get; set; }
		public bool IsFemale { get; set; }
		public DateTime LastSignIn { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
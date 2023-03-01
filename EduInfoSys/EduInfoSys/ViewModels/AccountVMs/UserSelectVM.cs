namespace EduInfoSys.ViewModels.AccountVMs
{
	public class UserSelectVM
	{
		public AppUser User { get; set; }
		public string Id { get => User.Id; }
		public string FullName { get => User.FirstName + " " + User.LastName; }
	}
}

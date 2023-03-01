namespace EduInfoSys.Models
{
	public class Customization
	{
		public int Id { get; set; }
		public string ColorScheme { get; set; }
		public string SidebarLayout { get; set; }
		public string SidebarPosition { get; set; }
		public string Layout { get; set; }
		public AppUser User { get; set; }
	}
}

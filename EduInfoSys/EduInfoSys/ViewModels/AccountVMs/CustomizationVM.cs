namespace EduInfoSys.ViewModels.AccountVMs
{
	public class CustomizationVM
	{
		[Required(ErrorMessage = "Theme is required")]
		public string ColorScheme { get; set; }
		[Required(ErrorMessage = "Sidebar layout is required")]
		public string SidebarLayout { get; set; }
		[Required(ErrorMessage = "Sidebar positions is required")]
		public string SidebarPosition { get; set; }
		[Required(ErrorMessage = "Layout is required")]
		public string Layout { get; set; }
	}
}

namespace EduInfoSys.Models
{
	public class Notification : BaseEntity
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public AppUser User { get; set; }
	}
}

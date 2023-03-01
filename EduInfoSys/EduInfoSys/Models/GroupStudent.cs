namespace EduInfoSys.Models
{
	public class GroupStudent : BaseEntity
	{
		public AppUser User { get; set; }
		public Group Group { get; set; }
	}
}

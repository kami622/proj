namespace EduInfoSys.Models
{
	public class Attendance : BaseEntity
	{
		public Subject Subject { get; set; }
		public DateTime Date { get; set; }
		public AppUser User { get; set; }
		public int Mark { get; set; }
		[MaxLength(100)]
		public string? Comment { get; set; }
	}
}

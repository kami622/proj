namespace EduInfoSys.ViewModels.SubjectVMs
{
	public class SubjectVM
	{
		public Subject Subject { get; set; }
		public List<SubjectNote> Notes { get; set; }
		public List<SubjectSource> Sources { get; set; }
		public List<TaskVM> TaskVMs { get; set; }
		public List<GroupStudent> Students { get; set; }
		public string MemberRole { get; set; }
	}
}

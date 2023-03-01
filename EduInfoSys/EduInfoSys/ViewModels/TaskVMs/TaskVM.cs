namespace EduInfoSys.ViewModels.TaskVMs
{
	public class TaskVM
	{
		public int Id { get => SubjectTask.Id; }
		public SubjectTask SubjectTask { get; set; }
		public StudentTask StudentTask { get; set; }
		public TaskGrade TaskGrade { get; set; }
	}
}

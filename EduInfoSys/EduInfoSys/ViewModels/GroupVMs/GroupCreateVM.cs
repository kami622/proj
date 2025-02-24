﻿namespace EduInfoSys.ViewModels.GroupVMs
{
	public class GroupCreateVM
	{
		[Required(ErrorMessage = "Name field is required"), MaxLength(100, ErrorMessage = "Name length must be less than 100 characters")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Department is required")]
		public int DepartmentId { get; set; }
	}
}

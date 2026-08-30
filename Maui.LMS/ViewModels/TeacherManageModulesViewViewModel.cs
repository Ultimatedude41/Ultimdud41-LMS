using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageModulesViewViewModel
	{
		private int courseId;

		public TeacherManageModulesViewViewModel(int courseId)
		{
			this.courseId = courseId;
		}

		public Course CurrentCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
			}
		}

		public List<Module> Modules
		{
			get
			{
				return CurrentCourse?.Modules ?? new List<Module>();
			}
		}

		public int GetModuleId(object module)
		{
			return (module as Module)?.Id ?? 0;
		}

		public void DeleteModule(object module)
		{
			var moduleToDelete = module as Module;
			if (moduleToDelete != null && CurrentCourse != null)
			{
				// Remove module from course
				CurrentCourse.Modules.Remove(moduleToDelete);

				// Remove module from service
				ModuleServiceProxy.Current.Modules.Remove(moduleToDelete);
			}
		}
	}
}
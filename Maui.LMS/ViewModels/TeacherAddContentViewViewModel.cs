using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherAddContentViewViewModel
	{
		private int courseId;

		public TeacherAddContentViewViewModel(int courseId)
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

		public int NextContentKey
		{
			get
			{
				return ModuleServiceProxy.Current.NextContentKey;
			}
		}

		public List<Assignment> Assignments
		{
			get
			{
				return CurrentCourse?.Assignments ?? new List<Assignment>();
			}
		}
	}
}
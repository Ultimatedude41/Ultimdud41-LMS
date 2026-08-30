using System;
using System.Collections.Generic;
using System.Linq;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentModulesViewViewModel
	{
		private int studentId;
		private int courseId;

		public StudentModulesViewViewModel(int studentId, int courseId)
		{
			this.studentId = studentId;
			this.courseId = courseId;
		}

		public Student CurrentStudent
		{
			get
			{
				return StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == studentId);
			}
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

		public int GetContentId(object content)
		{
			return (content as ContentPlus)?.Id ?? 0;
		}

		public string GetContentDisplay(object content)
		{
			return (content as ContentPlus)?.ToString() ?? "Unknown Content";
		}
	}
}

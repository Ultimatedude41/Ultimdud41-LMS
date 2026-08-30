using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageModuleContentViewViewModel
	{
		private int courseId;
		private int moduleId;

		public TeacherManageModuleContentViewViewModel(int courseId, int moduleId)
		{
			this.courseId = courseId;
			this.moduleId = moduleId;
		}

		public Course CurrentCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
			}
		}

		public Module CurrentModule
		{
			get
			{
				return ModuleServiceProxy.Current.Modules.FirstOrDefault(m => m.Id == moduleId);
			}
		}

		public List<ContentPlus> Content
		{
			get
			{
				return CurrentModule?.Content ?? new List<ContentPlus>();
			}
		}

		public int GetContentId(object content)
		{
			return (content as ContentPlus)?.Id ?? 0;
		}

		public void DeleteContent(object content)
		{
			var contentToDelete = content as ContentPlus;
			if (contentToDelete != null && CurrentModule != null)
			{
				CurrentModule.Content.Remove(contentToDelete);
			}
		}
	}
}
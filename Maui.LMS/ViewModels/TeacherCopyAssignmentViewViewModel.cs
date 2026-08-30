using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherCopyAssignmentViewViewModel
	{
		private int targetCourseId;

		public TeacherCopyAssignmentViewViewModel(int targetCourseId)
		{
			this.targetCourseId = targetCourseId;
		}

		public Course TargetCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == targetCourseId);
			}
		}

		public List<Course> OtherCourses
		{
			get
			{
				return CourseServiceProxy.Current.Courses.Where(c => c.Id != targetCourseId).ToList();
			}
		}

		public int GetAssignmentId(object assignment)
		{
			return (assignment as Assignment)?.Id ?? 0;
		}
	}
}
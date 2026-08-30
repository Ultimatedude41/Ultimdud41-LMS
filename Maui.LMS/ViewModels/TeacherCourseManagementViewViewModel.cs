using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherCourseManagementViewViewModel
	{
		public List<Course> Courses
		{
			get
			{
				return CourseServiceProxy.Current.Courses ?? new List<Course>();
			}
		}

		public int GetCourseId(object course)
		{
			return (course as Course)?.Id ?? 0;
		}

		public void DeleteCourse(object course)
		{
			var courseToDelete = course as Course;
			if (courseToDelete != null)
			{
				courseToDelete.Students.Clear();
				CourseServiceProxy.Current.Courses.Remove(courseToDelete);
			}
		}
	}
}
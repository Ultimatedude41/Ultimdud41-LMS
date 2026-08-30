using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentCourseViewViewModel
	{
		private int studentId;

		public StudentCourseViewViewModel(int studentId)
		{
			this.studentId = studentId;
		}

		public Student CurrentStudent
		{
			get
			{
				return StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == studentId);
			}
		}

		public List<Course> EnrolledCourses
		{
			get
			{
				return CourseServiceProxy.Current.Courses
					.Where(c => c.Students.Any(s => s.Id == CurrentStudent.Id))
					.ToList();
			}
		}

		public int GetCourseId(object course)
		{
			return (course as Course)?.Id ?? 0;
		}
	}
}

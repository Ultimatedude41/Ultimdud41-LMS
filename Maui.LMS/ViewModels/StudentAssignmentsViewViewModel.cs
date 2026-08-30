using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class StudentAssignmentsViewViewModel
	{
		private int studentId;
		private int courseId;

		public StudentAssignmentsViewViewModel(int studentId, int courseId)
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

		public List<Assignment> Assignments
		{
			get
			{
				return CurrentCourse?.Assignments ?? new List<Assignment>();
			}
		}

		public int GetAssignmentId(object assignment)
		{
			return (assignment as Assignment)?.Id ?? 0;
		}
	}
}

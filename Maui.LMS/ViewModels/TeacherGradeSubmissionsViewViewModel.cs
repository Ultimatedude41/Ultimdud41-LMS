using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherGradeSubmissionsViewViewModel
	{
		private int courseId;
		private int assignmentId;

		public TeacherGradeSubmissionsViewViewModel(int courseId, int assignmentId)
		{
			this.courseId = courseId;
			this.assignmentId = assignmentId;
		}

		public Course CurrentCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
			}
		}

		public Assignment CurrentAssignment
		{
			get
			{
				return AssignmentServiceProxy.Current.Assignments.FirstOrDefault(a => a.Id == assignmentId);
			}
		}

		public List<Submission> Submissions
		{
			get
			{
				return CurrentAssignment?.Submissions ?? new List<Submission>();
			}
		}

		public Student GetStudent(int studentId)
		{
			return StudentServiceProxy.Current.Students.FirstOrDefault(s => s.Id == studentId);
		}
	}
}

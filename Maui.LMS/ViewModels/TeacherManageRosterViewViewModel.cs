using System;
using System.Collections.Generic;
using System.Linq;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageRosterViewViewModel
	{
		private int courseId;

		public TeacherManageRosterViewViewModel(int courseId)
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

		public List<Student> EnrolledStudents
		{
			get
			{
				return CurrentCourse?.Students ?? new List<Student>();
			}
		}

		public List<Student> AvailableStudents
		{
			get
			{
				if (CurrentCourse == null)
					return StudentServiceProxy.Current.Students;

				return StudentServiceProxy.Current.Students
					.Where(s => !CurrentCourse.Students.Contains(s))
					.ToList();
			}
		}

		public void AddStudent(object student)
		{
			var studentToAdd = student as Student;
			if (studentToAdd != null && CurrentCourse != null)
			{
				CurrentCourse.Students.Add(studentToAdd);
			}
		}

		public void RemoveStudent(object student)
		{
			var studentToRemove = student as Student;
			if (studentToRemove != null && CurrentCourse != null)
			{
				CurrentCourse.Students.Remove(studentToRemove);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherStudentManagementViewViewModel
	{
		public List<Student> Students
		{
			get
			{
				return StudentServiceProxy.Current.Students ?? new List<Student>();
			}
		}

		public int GetStudentId(object student)
		{
			return (student as Student)?.Id ?? 0;
		}

		public void DeleteStudent(object student)
		{
			var studentToDelete = student as Student;
			if (studentToDelete != null)
			{
				// Remove student from all courses
				foreach (var course in CourseServiceProxy.Current.Courses)
				{
					course.Students.Remove(studentToDelete);
				}

				// Remove all submissions for this student
				var submissionsToRemove = SubmitServiceProxy.Current.Submissions
					.Where(s => s.StudentId == studentToDelete.Id)
					.ToList();

				foreach (var submission in submissionsToRemove)
				{
					SubmitServiceProxy.Current.Submissions.Remove(submission);
					var assignment = AssignmentServiceProxy.Current.Assignments
						.FirstOrDefault(a => a.Id == submission.AssignmentId);
					assignment?.Submissions.Remove(submission);
				}

				// Delete through service proxy which calls API
				StudentServiceProxy.Current.Delete(studentToDelete);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageAssignmentsViewViewModel
	{
		private int courseId;

		public TeacherManageAssignmentsViewViewModel(int courseId)
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

		public void DeleteAssignment(object assignment)
		{
			var assignmentToDelete = assignment as Assignment;
			if (assignmentToDelete != null && CurrentCourse != null)
			{
				// Remove all submissions for this assignment
				var submissionsToRemove = assignmentToDelete.Submissions.ToList();
				foreach (var submission in submissionsToRemove)
				{
					SubmitServiceProxy.Current.Submissions.Remove(submission);
					assignmentToDelete.Submissions.Remove(submission);
				}

				// Remove assignment from course
				CurrentCourse.Assignments.Remove(assignmentToDelete);

				// Remove assignment from service
				AssignmentServiceProxy.Current.Assignments.Remove(assignmentToDelete);

				// Remove assignment from its assignment group
				foreach (var agroup in AGroupServiceProxy.Current.AGroups)
				{
					agroup.Assignments.Remove(assignmentToDelete);
				}
			}
		}
	}
}

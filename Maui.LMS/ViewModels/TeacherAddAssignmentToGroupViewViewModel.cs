using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherAddAssignmentToGroupViewViewModel
	{
		private int courseId;
		private int groupId;

		public TeacherAddAssignmentToGroupViewViewModel(int courseId, int groupId)
		{
			this.courseId = courseId;
			this.groupId = groupId;
		}

		public Course CurrentCourse
		{
			get
			{
				return CourseServiceProxy.Current.Courses.FirstOrDefault(c => c.Id == courseId);
			}
		}

		public AGroup CurrentGroup
		{
			get
			{
				return AGroupServiceProxy.Current.AGroups.FirstOrDefault(g => g.Id == groupId);
			}
		}

		public List<Assignment> AvailableAssignments
		{
			get
			{
				if (CurrentCourse == null || CurrentGroup == null)
					return new List<Assignment>();

				// Return assignments that are not already in this group
				return CurrentCourse.Assignments
					.Where(a => !CurrentGroup.Assignments.Contains(a))
					.ToList();
			}
		}
	}
}
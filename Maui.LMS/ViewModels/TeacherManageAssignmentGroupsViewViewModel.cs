using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageAssignmentGroupsViewViewModel
	{
		private int courseId;

		public TeacherManageAssignmentGroupsViewViewModel(int courseId)
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

		public List<AGroup> AssignmentGroups
		{
			get
			{
				return CurrentCourse?.AGroups ?? new List<AGroup>();
			}
		}

		public int GetGroupId(object group)
		{
			return (group as AGroup)?.Id ?? 0;
		}

		public void DeleteGroup(object group)
		{
			var groupToDelete = group as AGroup;
			if (groupToDelete != null && CurrentCourse != null)
			{
				CurrentCourse.AGroups.Remove(groupToDelete);
				AGroupServiceProxy.Current.AGroups.Remove(groupToDelete);
			}
		}
	}
}
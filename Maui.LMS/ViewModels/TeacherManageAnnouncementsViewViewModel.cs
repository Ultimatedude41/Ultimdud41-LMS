using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels
{
	public class TeacherManageAnnouncementsViewViewModel
	{
		private int courseId;

		public TeacherManageAnnouncementsViewViewModel(int courseId)
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

		public List<Announcement> Announcements
		{
			get
			{
				return CurrentCourse?.Announcements ?? new List<Announcement>();
			}
		}

		public int GetAnnouncementId(object announcement)
		{
			return (announcement as Announcement)?.Id ?? 0;
		}

		public void DeleteAnnouncement(object announcement)
		{
			var announcementToDelete = announcement as Announcement;
			if (announcementToDelete != null && CurrentCourse != null)
			{
				CurrentCourse.Announcements.Remove(announcementToDelete);
				AnnouncementServiceProxy.Current.Announcements.Remove(announcementToDelete);
			}
		}
	}
}
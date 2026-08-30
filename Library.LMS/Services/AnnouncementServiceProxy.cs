using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
	public class AnnouncementServiceProxy
	{
		private static object _lock = new object();
		private static AnnouncementServiceProxy? instance;
		public static AnnouncementServiceProxy Current
		{
			get
			{
				lock (_lock)
				{
					if (instance == null)
					{
						instance = new AnnouncementServiceProxy();
					}
				}
				return instance;
			}
		}

		private AnnouncementServiceProxy()
		{
			Announcements = new List<Announcement>();
		}

		public List<Announcement> Announcements { get; private set; }

		public void Add(Announcement announcement)
		{
			if (announcement != null)
			{
				Announcements.Add(announcement);
			}
		}
	}
}
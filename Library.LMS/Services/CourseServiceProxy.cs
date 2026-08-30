using System;
using System.Collections.Generic;
using System.Text;
using CLI.LMS.Model;

namespace Library.LMS.Services
{
    public class CourseServiceProxy
    {
		private List<Course> courses;

		public List<Course> Courses
		{
			get
			{
				return courses;
			}

			set
			{
				if (courses != value)
				{
					courses = value;
				}
			}
		}
		private static CourseServiceProxy? instance;
		private static object instanceLock = new object();

		public static CourseServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new CourseServiceProxy();
					}
				}
				return instance;
			}
		}
		private CourseServiceProxy()
		{
			var assignments = AssignmentServiceProxy.Current.Assignments;
			var modules = ModuleServiceProxy.Current.Modules;
			var agroups = AGroupServiceProxy.Current.AGroups;
			var announcements = AnnouncementServiceProxy.Current.Announcements;
			courses = new List<Course>
			{
				new Course { Id = 1, Name = "Introduction to Programming", Code = "CS101", Semester = "Spring 2026", Section = "A", Description = "Learn the basics of programming", Modules = new List<Module> { modules[0] }, Assignments = new List<Assignment> { assignments[0], assignments[3], assignments[6] }, AGroups = new List<AGroup> { agroups[0], agroups[3], agroups[6] }  },
				new Course { Id = 2, Name = "Chemistry I", Code = "CHEM101", Semester = "Spring 2026", Section = "B", Description = "Properties of atoms and physical states of matter", Modules = new List<Module> { modules[1] }, Assignments = new List<Assignment> { assignments[1], assignments[4], assignments[7] }, AGroups = new List<AGroup> { agroups[1], agroups[4], agroups[7] }  },
				new Course { Id = 3, Name = "Elementary Japanese I", Code = "JPN112", Semester = "Fall 2025", Section = "A", Description = "Fundamental Japanese grammar, vocabulary, and writing", Modules = new List<Module> { modules[2] }, Assignments = new List<Assignment> { assignments[2], assignments[5], assignments[8] }, AGroups = new List<AGroup> { agroups[2], agroups[5], agroups[8] }  }
			};

			// Add welcome announcements to all preloaded courses
			foreach (var course in courses)
			{
				var welcomeAnnouncement = new Announcement
				{
					Title = $"Welcome to {course.Name}!",
					Message = "Check this space regularly for important updates and announcements throughout the semester.",
					PostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				};
				AnnouncementServiceProxy.Current.Add(welcomeAnnouncement);
				course.Announcements.Add(welcomeAnnouncement);
			}

		}

		public void Add(Course course)
		{
			course.Id = NextKey;
			Courses.Add(course);

			// Add welcome announcement to new course
			var welcomeAnnouncement = new Announcement
			{
				Title = $"Welcome to {course.Name}!",
				Message = "Check this space regularly for important updates and announcements throughout the semester.",
				PostDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
			};
			AnnouncementServiceProxy.Current.Add(welcomeAnnouncement);
			course.Announcements.Add(welcomeAnnouncement);
		}

		public int NextKey
		{
			get
			{
				if (Courses.Any())
				{
					return Courses.Select(i => i.Id).Max() + 1;
				}
				return 1;
			}
		}

	}
}

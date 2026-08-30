using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
    public class Course
    {
		private int id;
		public int Id
		{
			get
			{
				return id;
			}
			set
			{
				if (id != value)
				{
					id = value;
				}
			}

		}

		public string Name { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public string Semester { get; set; }
		public string Section { get; set; }
		public double GradeRangeA { get; set; } = 90;
		public double GradeRangeB { get; set; } = 80;
		public double GradeRangeC { get; set; } = 70;
		public double GradeRangeD { get; set; } = 60;
		public List<Module> Modules { get; set; } = new List<Module>();
		public List<Assignment> Assignments { get; set; } = new List<Assignment>();
		public List<Student> Students { get; set; } = new List<Student>();
		public List<AGroup> AGroups { get; set; } = new List<AGroup>();
		public List<Announcement> Announcements { get; set; } = new List<Announcement>();
		public override string ToString()
		{
			var formattedSection = !string.IsNullOrWhiteSpace(Section) ? $" Section {Section}" : "";
			return $"{Id} [{Code}]{formattedSection} {Name} ({Semester}) - {Description}";
		}

		public string Display => ToString() ?? string.Empty;

		public Course()
		{
		}
	}
}

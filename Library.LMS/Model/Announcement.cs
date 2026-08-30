using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
	public class Announcement
	{
		private static int lastId = 1;
		public int Id { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string PostDate { get; set; }

		public Announcement()
		{
			Id = lastId++;
		}

		public override string ToString()
		{
			return $"{Title} - {PostDate}";
		}
	}
}

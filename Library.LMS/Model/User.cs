using System;
using System.Collections.Generic;
using System.Text;
namespace CLI.LMS.Model
{
	public class User  // Changed from internal to public
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
		public string Code { get; set; } // Student ID number (like "S12345")
	}
}

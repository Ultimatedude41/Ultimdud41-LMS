using System;
using System.Collections.Generic;
using System.Text;
namespace CLI.LMS.Model
{
	public class Student : User
	{
		public String Classification { get; set; }

		public override string ToString()
		{
			return $"[{Id}] {Code} - {Name} ({Classification})";
		}

		public string Display => ToString() ?? string.Empty;
	}
}

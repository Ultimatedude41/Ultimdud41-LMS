using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
	public class Quiz : Assignment
	{
		public string Question { get; set; }

		public override string ToString()
		{
			return $"[{Id}] [Quiz] {Name} (Points: {AvailablePoints}, Due: {DueDate})";
		}

		public string Display => ToString() ?? string.Empty;
	}
}
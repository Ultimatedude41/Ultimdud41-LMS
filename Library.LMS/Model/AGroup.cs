using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
	public class AGroup
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Weight { get; set; }  // ADD THIS - percentage weight (e.g., 0.25 = 25%)
		public List<Assignment> Assignments { get; set; } = new List<Assignment>();

		public override string ToString()
		{
			return $"[{Id}] {Name} - {Assignments.Count} assignment(s) - Weight: {Weight * 100}%";
		}
	}
}
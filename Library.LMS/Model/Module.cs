using System;
using System.Collections.Generic;
using System.Text;

namespace CLI.LMS.Model
{
	public class Module
	{
		public int Id { get; set; }
		public List<ContentPlus> Content { get; set; } = new List<ContentPlus>();

		public override string ToString()
		{
			return $"Module {Id} - {Content.Count} item(s)";
		}

		public string Display => ToString() ?? string.Empty;
	}
}